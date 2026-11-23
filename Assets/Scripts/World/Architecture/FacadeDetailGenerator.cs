using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    /// <summary>
    /// Gerador de detalhes de fachadas brasileiras para RERY CITY.
    /// Acrescenta muros frontais, portões de garagem, letreiros comerciais e detalhes
    /// arquitetônicos determinísticos respeitando o recuo do lote.
    /// </summary>
    [AddComponentMenu("Rery City/Architecture/Facade Detail Generator")]
    public class FacadeDetailGenerator : MonoBehaviour
    {
        public static FacadeDetailGenerator Instance { get; private set; }

        [Header("Catálogo de Letreiros")]
        [SerializeField] private CommercialSignboardCatalog signboardCatalog;
        [SerializeField] private NeighborhoodThemeCatalog themeCatalog;

        [Header("Instâncias de Detalhe")]
        [SerializeField] private List<GameObject> activeDetailObjects = new List<GameObject>();

        public int DetailObjectCount => activeDetailObjects.Count;

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
        /// Aplica detalhes arquitetônicos sobre uma edificação gerada em um lote.
        /// </summary>
        public void ApplyFacadeDetails(GameObject buildingObj, UrbanLot lot, string neighborhoodName, int seed)
        {
            if (buildingObj == null || lot == null) return;

            Random.InitState(seed + lot.lotId.GetHashCode());

            // 1. Muro e Portão Frontal para Residências ou Oficinas
            if (lot.styleCategory == BuildingStyleCategory.ResidenciaPopular ||
                lot.styleCategory == BuildingStyleCategory.CasaPadraoMedio ||
                lot.styleCategory == BuildingStyleCategory.OficinaMecanicaGrau)
            {
                CreateFrontWallAndGate(buildingObj, lot);
            }

            // 2. Letreiro Comercial para Lojas, Oficinas e Postos
            if (lot.styleCategory == BuildingStyleCategory.EdificioComercial ||
                lot.styleCategory == BuildingStyleCategory.OficinaMecanicaGrau ||
                lot.styleCategory == BuildingStyleCategory.PostoCombustivel)
            {
                CreateSignboard(buildingObj, lot, seed);
            }
        }

        private void CreateFrontWallAndGate(GameObject buildingObj, UrbanLot lot)
        {
            // Cria muro na linha do recuo frontal (sem invadir a calçada)
            GameObject wallRoot = new GameObject($"Wall_{lot.lotId}");
            wallRoot.transform.SetParent(buildingObj.transform);

            // Muro frontal tem a largura da testada com vão de portão de 3.5m
            float wallThickness = 0.25f;
            float wallHeight = 2.2f;
            float gateWidth = 3.5f;
            float sideWallWidth = Mathf.Max(1.0f, (lot.frontageMeters - gateWidth) * 0.5f);

            // Metade esquerda do muro
            GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWall.name = "LeftWall";
            leftWall.transform.SetParent(wallRoot.transform);
            leftWall.transform.localScale = new Vector3(sideWallWidth, wallHeight, wallThickness);
            leftWall.transform.localPosition = new Vector3(-((gateWidth * 0.5f) + (sideWallWidth * 0.5f)), wallHeight * 0.5f, -(lot.depthMeters * 0.48f));

            // Metade direita do muro
            GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "RightWall";
            rightWall.transform.SetParent(wallRoot.transform);
            rightWall.transform.localScale = new Vector3(sideWallWidth, wallHeight, wallThickness);
            rightWall.transform.localPosition = new Vector3((gateWidth * 0.5f) + (sideWallWidth * 0.5f), wallHeight * 0.5f, -(lot.depthMeters * 0.48f));

            activeDetailObjects.Add(wallRoot);
        }

        private void CreateSignboard(GameObject buildingObj, UrbanLot lot, int seed)
        {
            if (signboardCatalog == null) return;

            CommercialSignboard sign = signboardCatalog.GetSignboardForCategory(lot.styleCategory, seed);

            GameObject signObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            signObj.name = $"Sign_{sign.establishmentId}_{lot.lotId}";
            signObj.transform.SetParent(buildingObj.transform);

            // Dimensões do letreiro (placa horizontal acima da entrada)
            float signWidth = Mathf.Min(lot.frontageMeters * 0.6f, 8f);
            float signHeight = 1.2f;
            float signDepth = 0.15f;

            signObj.transform.localScale = new Vector3(signWidth, signHeight, signDepth);
            signObj.transform.localPosition = new Vector3(0f, 3.8f, -(lot.depthMeters * 0.51f));

            activeDetailObjects.Add(signObj);
        }

        public void ClearDetails()
        {
            for (int i = activeDetailObjects.Count - 1; i >= 0; i--)
            {
                if (activeDetailObjects[i] != null)
                {
                    DestroyImmediate(activeDetailObjects[i]);
                }
            }
            activeDetailObjects.Clear();
        }
    }
}
