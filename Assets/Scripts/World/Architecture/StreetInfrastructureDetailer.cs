using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.World.Geography;

namespace ProjetoGrau2.World.Architecture
{
    public enum StreetItemType
    {
        MeioFioGuia,
        FaixaPedestreHorizontal,
        FaixaRetencaoSemaforo,
        LombadaRedutorVelocidade,
        PlacaNumeracaoImovel
    }

    [System.Serializable]
    public struct StreetDetailItem
    {
        public string detailId;
        public StreetItemType itemType;
        public Vector3 position;
        public Vector3 size;
        public float rotationY;
        public string roadName;
    }

    /// <summary>
    /// Gerencia a infraestrutura detalhada das vias e calçadas de Araguari para RERY CITY.
    /// Posiciona meio-fios, faixas de retenção, lombadas e placas de numeração predial.
    /// </summary>
    [AddComponentMenu("Rery City/Architecture/Street Infrastructure Detailer")]
    public class StreetInfrastructureDetailer : MonoBehaviour
    {
        public static StreetInfrastructureDetailer Instance { get; private set; }

        [Header("Itens Viários Cadastrados")]
        [SerializeField] private List<StreetDetailItem> registeredDetails = new List<StreetDetailItem>();

        public IReadOnlyList<StreetDetailItem> RegisteredDetails => registeredDetails;
        public int DetailCount => registeredDetails.Count;

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
        /// Gera a infraestrutura de calçadas, meio-fio e sinalização para um segmento viário real.
        /// </summary>
        public void DetailRoadSegment(RealRoadSegment road)
        {
            if (road == null) return;

            Vector3 center = road.transform.position;
            float halfWidth = road.RoadWidthMeters * 0.5f;

            // 1. Meio-fio esquerdo e direito
            StreetDetailItem curbLeft = new StreetDetailItem
            {
                detailId = $"Curb_L_{road.RoadName}_{registeredDetails.Count}",
                itemType = StreetItemType.MeioFioGuia,
                position = center + new Vector3(-halfWidth, 0.1f, 0f),
                size = new Vector3(0.2f, 0.2f, 20f),
                rotationY = 0f,
                roadName = road.RoadName
            };

            StreetDetailItem curbRight = new StreetDetailItem
            {
                detailId = $"Curb_R_{road.RoadName}_{registeredDetails.Count}",
                itemType = StreetItemType.MeioFioGuia,
                position = center + new Vector3(halfWidth, 0.1f, 0f),
                size = new Vector3(0.2f, 0.2f, 20f),
                rotationY = 0f,
                roadName = road.RoadName
            };

            registeredDetails.Add(curbLeft);
            registeredDetails.Add(curbRight);

            // 2. Lombada em vias coletoras ou locais (quando aplicável)
            if (road.Classification == RoadClassification.ColetoraUrbana ||
                road.Classification == RoadClassification.LocalComercial)
            {
                StreetDetailItem speedBump = new StreetDetailItem
                {
                    detailId = $"Bump_{road.RoadName}_{registeredDetails.Count}",
                    itemType = StreetItemType.LombadaRedutorVelocidade,
                    position = center,
                    size = new Vector3(road.RoadWidthMeters, 0.10f, 1.5f), // 10cm altura suave para motos
                    rotationY = 0f,
                    roadName = road.RoadName
                };
                registeredDetails.Add(speedBump);
            }

            // 3. Faixa de pedestres se configurada no segmento
            if (road.HasPedestrianCrossing)
            {
                StreetDetailItem crossing = new StreetDetailItem
                {
                    detailId = $"Crossing_{road.RoadName}_{registeredDetails.Count}",
                    itemType = StreetItemType.FaixaPedestreHorizontal,
                    position = center + new Vector3(0f, 0.02f, 10f),
                    size = new Vector3(road.RoadWidthMeters, 0.02f, 3.0f),
                    rotationY = 0f,
                    roadName = road.RoadName
                };
                registeredDetails.Add(crossing);
            }
        }

        public void ClearDetails()
        {
            registeredDetails.Clear();
        }
    }
}
