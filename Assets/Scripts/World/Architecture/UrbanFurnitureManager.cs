using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.World.Geography;

namespace ProjetoGrau2.World.Architecture
{
    public enum FurnitureType
    {
        PosteIluminacao,
        PlacaTransito,
        FaixaPedestre,
        PontoOnibus,
        BancoPraca,
        LixeiraUrbana,
        ArvoreUrbana,
        MuroDivisorio
    }

    [System.Serializable]
    public struct UrbanFurnitureItem
    {
        public string itemId;
        public FurnitureType furnitureType;
        public Vector3 worldPosition;
        public float rotationY;
        public string associatedRoadOrPoi;
    }

    /// <summary>
    /// Gerenciador de mobiliário urbano, vegetação e sinalização para RERY CITY.
    /// Distribui postes, placas, paradas de ônibus e árvores ao longo das calçadas e praças reais.
    /// </summary>
    [AddComponentMenu("Rery City/Architecture/Urban Furniture Manager")]
    public class UrbanFurnitureManager : MonoBehaviour
    {
        public static UrbanFurnitureManager Instance { get; private set; }

        [Header("Itens de Mobiliário Cadastrados")]
        [SerializeField] private List<UrbanFurnitureItem> furnitureItems = new List<UrbanFurnitureItem>();

        [Header("Instâncias em Cena")]
        [SerializeField] private List<GameObject> activeInstances = new List<GameObject>();

        public IReadOnlyList<UrbanFurnitureItem> FurnitureItems => furnitureItems;
        public int ActiveInstanceCount => activeInstances.Count;

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
        /// Cadastra um elemento de mobiliário urbano na malha.
        /// </summary>
        public void RegisterFurniture(UrbanFurnitureItem item)
        {
            furnitureItems.Add(item);
        }

        /// <summary>
        /// Gera automaticamente postes ao longo de um segmento viário real.
        /// </summary>
        public void GenerateStreetLightsForRoad(RealRoadSegment road, float spacingMeters = 30f)
        {
            if (road == null) return;

            Vector3 startPos = road.transform.position;
            // Gera postes nas duas margens das calçadas
            float halfWidth = (road.RoadWidthMeters * 0.5f) + (road.SidewalkWidthMeters * 0.5f);

            UrbanFurnitureItem leftLight = new UrbanFurnitureItem
            {
                itemId = $"Light_L_{road.RoadName}_{furnitureItems.Count}",
                furnitureType = FurnitureType.PosteIluminacao,
                worldPosition = startPos + new Vector3(-halfWidth, 0f, 0f),
                rotationY = 0f,
                associatedRoadOrPoi = road.RoadName
            };

            UrbanFurnitureItem rightLight = new UrbanFurnitureItem
            {
                itemId = $"Light_R_{road.RoadName}_{furnitureItems.Count}",
                furnitureType = FurnitureType.PosteIluminacao,
                worldPosition = startPos + new Vector3(halfWidth, 0f, 0f),
                rotationY = 180f,
                associatedRoadOrPoi = road.RoadName
            };

            furnitureItems.Add(leftLight);
            furnitureItems.Add(rightLight);
        }
    }
}
