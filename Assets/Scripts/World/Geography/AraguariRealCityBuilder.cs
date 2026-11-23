using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ProjetoGrau2.World.Architecture;

namespace ProjetoGrau2.World.Geography
{
    /// <summary>
    /// Construtor de mundo georreferenciado para Araguari (MG).
    /// Carrega o dataset de geometria real do IBGE Censo 2022 (offline via StreamingAssets)
    /// com suporte a UnityWebRequest no Android (dentro do APK/JAR) e File.ReadAllText no Desktop.
    /// Orquestra a geração de quadras, corredores viários e edificações nos contornos exatos.
    /// </summary>
    [AddComponentMenu("Rery City/Geography/Araguari Real City Builder")]
    public class AraguariRealCityBuilder : MonoBehaviour
    {
        public static AraguariRealCityBuilder Instance { get; private set; }

        [Header("Configuração de Arquivos")]
        [SerializeField] private string jsonRelativePath = "Geography/araguari_pilot_geometry.json";
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool buildGeometryOnLoad = true;

        [Header("Referências de Geração")]
        [SerializeField] private Transform quadrasRoot;
        [SerializeField] private Transform roadsRoot;
        [SerializeField] private BuildingArchetypeCatalog archetypeCatalog;
        [SerializeField] private CityMaterialLibrary materialLibrary;

        [Header("Configuração de Superfície e Calçadas")]
        [SerializeField] private Material roadAsphaltMaterial;
        [SerializeField] private Material sidewalkMaterial;
        [SerializeField] private float blockHeightMeters = 0.2f; // Altura do meio-fio/calçada

        [Header("Estado dos Dados")]
        [SerializeField] private bool isLoaded = false;
        private RealCityGeometryPayload _payload;
        private readonly Dictionary<string, RealQuadraData> _quadraMap = new Dictionary<string, RealQuadraData>();
        private readonly List<RealRoadCorridor> _corridors = new List<RealRoadCorridor>();

        public bool IsLoaded => isLoaded;
        public RealCityGeometryPayload Payload => _payload;
        public CityGeoMetadata Metadata => _payload?.metadata;
        public IReadOnlyList<RealQuadraData> Quadras => _payload?.quadras;
        public IReadOnlyList<RoadCorridorData> CorridorsData => _payload?.corridors;
        public IReadOnlyList<IntersectionData> Intersections => _payload?.intersections;
        public IReadOnlyList<string> StreetNames => _payload?.streets;
        public IReadOnlyList<RealFaceData> Faces => _payload?.faces;
        public IReadOnlyList<RealRoadCorridor> ActiveCorridors => _corridors;

        public event Action OnCityDataLoaded;
        public event Action OnGeometryBuilt;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (autoLoadOnStart && !isLoaded)
            {
                StartCoroutine(LoadCityGeometryCoroutine());
            }
        }

        public IEnumerator LoadCityGeometryCoroutine(string overrideJsonPath = null)
        {
            string filePath = overrideJsonPath;
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(Application.streamingAssetsPath, jsonRelativePath);
            }

            string jsonContent = null;

            // Suporte multiplataforma: UnityWebRequest para Android (arquivos empacotados em APK/JAR)
            if (filePath.Contains("://") || filePath.Contains("jar:file"))
            {
                using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        jsonContent = uwr.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"[AraguariRealCityBuilder] Erro UnityWebRequest ao ler {filePath}: {uwr.error}");
                    }
                }
            }
            else
            {
                // Desktop / Editor / Standalone Windows
                if (File.Exists(filePath))
                {
                    jsonContent = File.ReadAllText(filePath);
                }
                else
                {
                    Debug.LogWarning($"[AraguariRealCityBuilder] Arquivo não encontrado: {filePath}");
                }
            }

            if (!string.IsNullOrEmpty(jsonContent))
            {
                bool success = ParseCityGeometryJson(jsonContent);
                if (success && buildGeometryOnLoad)
                {
                    BuildPhysicalEnvironment();
                }
            }
        }

        public bool ParseCityGeometryJson(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText)) return false;

            try
            {
                _payload = JsonUtility.FromJson<RealCityGeometryPayload>(jsonText);
                if (_payload == null || _payload.quadras == null)
                {
                    Debug.LogError("[AraguariRealCityBuilder] Falha ao desserializar JSON de geometria real.");
                    return false;
                }

                _quadraMap.Clear();
                for (int i = 0; i < _payload.quadras.Count; i++)
                {
                    var q = _payload.quadras[i];
                    _quadraMap[q.quadra_id] = q;
                }

                isLoaded = true;
                Debug.Log($"[AraguariRealCityBuilder] Geometria de Araguari carregada: {_payload.quadras.Count} quadras, {_payload.streets.Count} ruas, {_payload.faces.Count} faces.");
                OnCityDataLoaded?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AraguariRealCityBuilder] Exceção no parse do JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Constrói os GameObjects, colliders e corredores viários na cena com base nos dados reais do IBGE.
        /// </summary>
        public void BuildPhysicalEnvironment()
        {
            if (!isLoaded || _payload == null) return;

            if (roadsRoot == null)
            {
                var roadsGo = new GameObject("Araguari_Real_Roads_Root");
                roadsGo.transform.SetParent(transform, false);
                roadsRoot = roadsGo.transform;
            }

            if (quadrasRoot == null)
            {
                var quadrasGo = new GameObject("Araguari_Real_Quadras_Root");
                quadrasGo.transform.SetParent(transform, false);
                quadrasRoot = quadrasGo.transform;
            }

            // 1. Instanciação dos Corredores Viários
            _corridors.Clear();
            if (_payload.corridors != null)
            {
                for (int i = 0; i < _payload.corridors.Count; i++)
                {
                    var cData = _payload.corridors[i];
                    var roadGo = new GameObject($"Road_{cData.street_name.Replace(' ', '_')}");
                    roadGo.transform.SetParent(roadsRoot, false);

                    var corridor = roadGo.AddComponent<RealRoadCorridor>();
                    corridor.InitializeCorridor(
                        cData.street_name,
                        cData.neighborhood,
                        cData.road_width_meters,
                        cData.lane_count,
                        false,
                        cData.speed_limit_kmh
                    );

                    var nodes = new List<Vector3>();
                    if (cData.centerline_nodes != null)
                    {
                        for (int n = 0; n < cData.centerline_nodes.Count; n++)
                        {
                            nodes.Add(cData.centerline_nodes[n].ToVector3(0f));
                        }
                    }
                    corridor.SetCenterline(nodes);
                    _corridors.Add(corridor);
                }
            }

            // 2. Instanciação das Quadras com Colisores Físicos
            if (_payload.quadras != null)
            {
                for (int i = 0; i < _payload.quadras.Count; i++)
                {
                    var qData = _payload.quadras[i];
                    var qGo = new GameObject($"Quadra_{qData.quadra_num}_{qData.quadra_id}");
                    qGo.transform.SetParent(quadrasRoot, false);
                    qGo.transform.position = new Vector3(qData.center.x, 0f, qData.center.z);

                    // Colisor da base da quadra/calçada
                    var boxCol = qGo.AddComponent<BoxCollider>();
                    boxCol.size = new Vector3(qData.bounds.width_meters, blockHeightMeters, qData.bounds.length_meters);
                    boxCol.center = new Vector3(0f, blockHeightMeters / 2.0f, 0f);
                }
            }

            Debug.Log($"[AraguariRealCityBuilder] Ambiente físico gerado: {_corridors.Count} corredores viários e {_payload.quadras.Count} quadras com colisores.");
            OnGeometryBuilt?.Invoke();
        }

        public RealQuadraData GetQuadraById(string quadraId)
        {
            if (_quadraMap.TryGetValue(quadraId, out var q))
            {
                return q;
            }
            return null;
        }

        public List<RealQuadraData> GetQuadrasInArea(float minX, float maxX, float minZ, float maxZ)
        {
            var result = new List<RealQuadraData>();
            if (_payload?.quadras == null) return result;

            for (int i = 0; i < _payload.quadras.Count; i++)
            {
                var q = _payload.quadras[i];
                if (q.bounds != null)
                {
                    if (q.bounds.max_x >= minX && q.bounds.min_x <= maxX &&
                        q.bounds.max_z >= minZ && q.bounds.min_z <= maxZ)
                    {
                        result.Add(q);
                    }
                }
            }
            return result;
        }

        public List<RealFaceData> GetFacesByStreet(string streetName)
        {
            var result = new List<RealFaceData>();
            if (_payload?.faces == null || string.IsNullOrEmpty(streetName)) return result;

            for (int i = 0; i < _payload.faces.Count; i++)
            {
                var f = _payload.faces[i];
                if (string.Equals(f.street_name, streetName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(f);
                }
            }
            return result;
        }

        public void SetPayloadForTesting(RealCityGeometryPayload testPayload)
        {
            _payload = testPayload;
            _quadraMap.Clear();
            if (_payload?.quadras != null)
            {
                for (int i = 0; i < _payload.quadras.Count; i++)
                {
                    _quadraMap[_payload.quadras[i].quadra_id] = _payload.quadras[i];
                }
            }
            isLoaded = true;
        }
    }
}
