using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.World;
using ProjetoGrau2.World.Geography;

namespace ProjetoGrau2.World.Architecture
{
    /// <summary>
    /// Gerador de construções e fachadas urbanas modulares para RERY CITY.
    /// Gera edifícios respeitando os lotes e recuos das vias reais de Araguari,
    /// atribuindo LODs físicos e materiais compartilhados automaticamente.
    /// </summary>
    [AddComponentMenu("Rery City/Architecture/Building Generator")]
    public class BuildingGenerator : MonoBehaviour
    {
        public static BuildingGenerator Instance { get; private set; }

        [Header("Configuração de Arquétipos e Semente")]
        [SerializeField] private BuildingArchetypeCatalog archetypeCatalog;
        [SerializeField] private int generationSeed = 839210;

        [Header("Lotes e Edifícios Gerados")]
        [SerializeField] private List<UrbanLot> definedLots = new List<UrbanLot>();
        [SerializeField] private List<GameObject> instantiatedBuildings = new List<GameObject>();

        [Header("Configuração de LOD e Performance")]
        [SerializeField] private float lod0Dist = 40f;
        [SerializeField] private float lod1Dist = 90f;
        [SerializeField] private float lod2Dist = 160f;

        public IReadOnlyList<UrbanLot> DefinedLots => definedLots;
        public int BuildingCount => instantiatedBuildings.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Gera edifícios determinísticos para todos os lotes definidos.
        /// </summary>
        public void GenerateCityBuildings(Transform parentRoot = null)
        {
            ClearBuildings();

            Random.InitState(generationSeed);
            Transform root = parentRoot != null ? parentRoot : transform;

            for (int i = 0; i < definedLots.Count; i++)
            {
                UrbanLot lot = definedLots[i];
                if (lot == null) continue;

                BuildingArchetype arch = archetypeCatalog != null 
                    ? archetypeCatalog.GetArchetypeForCategory(lot.styleCategory)
                    : new BuildingArchetype();

                GameObject bldg = CreateBuildingInstance(lot, arch, root);
                if (bldg != null)
                {
                    instantiatedBuildings.Add(bldg);
                }
            }
        }

        private GameObject CreateBuildingInstance(UrbanLot lot, BuildingArchetype arch, Transform parent)
        {
            int floorCount = Random.Range(arch.minFloors, arch.maxFloors + 1);
            float totalHeight = Mathf.Max(3f, floorCount * (arch.floorHeight > 0f ? arch.floorHeight : 3.0f));

            float bldgWidth = Mathf.Clamp(lot.frontageMeters * 0.85f, arch.minWidth, arch.maxWidth > 0f ? arch.maxWidth : 30f);
            float bldgDepth = Mathf.Clamp(lot.depthMeters * 0.75f, arch.minDepth, arch.maxDepth > 0f ? arch.maxDepth : 30f);

            GameObject bldgObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bldgObj.name = $"Building_{lot.styleCategory}_{lot.lotId}";
            bldgObj.transform.SetParent(parent);

            // Posicionamento ajustado ao piso
            Vector3 spawnPos = lot.centerPosition + new Vector3(0f, totalHeight * 0.5f, 0f);
            bldgObj.transform.position = spawnPos;
            bldgObj.transform.rotation = Quaternion.Euler(0f, lot.rotationY, 0f);
            bldgObj.transform.localScale = new Vector3(bldgWidth, totalHeight, bldgDepth);

            // Configurar LOD
            WorldLODGroup lodGroup = bldgObj.AddComponent<WorldLODGroup>();
            lodGroup.InitializeDistances(lod0Dist, lod1Dist, lod2Dist);

            return bldgObj;
        }

        /// <summary>
        /// Adiciona um lote urbano à lista de geração.
        /// </summary>
        public void RegisterLot(UrbanLot lot)
        {
            if (lot != null && !definedLots.Contains(lot))
            {
                definedLots.Add(lot);
            }
        }

        /// <summary>
        /// Limpa construções geradas.
        /// </summary>
        public void ClearBuildings()
        {
            for (int i = instantiatedBuildings.Count - 1; i >= 0; i--)
            {
                if (instantiatedBuildings[i] != null)
                {
                    DestroyImmediate(instantiatedBuildings[i]);
                }
            }
            instantiatedBuildings.Clear();
        }
    }
}
