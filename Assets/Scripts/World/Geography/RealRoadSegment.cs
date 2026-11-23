using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.World.Geography
{
    public enum RoadClassification
    {
        ArterialDupla,     // ex: Av. Cel. Theodolino, Av. Minas Gerais
        ColetoraUrbana,    // ex: Av. Mato Grosso, Rua Brasil Accioly
        LocalComercial,    // ex: Rua Marciano Santos, Rua Rui Barbosa
        RotatoriaPraca,    // ex: Contorno Praça Manoel Bonito
        AcessoBairro       // ex: Ruas de ligação Centro-Bosque-Goiás
    }

    /// <summary>
    /// Representa um segmento viário real de Araguari georreferenciado, com metadados de tráfego,
    /// largura, calçadas, faixas de pedestres e nós de navegação compatíveis com o TrafficNavigationManager.
    /// </summary>
    [AddComponentMenu("Rery City/Geography/Real Road Segment")]
    public class RealRoadSegment : MonoBehaviour
    {
        [Header("Identificação Cartográfica")]
        [SerializeField] private string roadName = "Avenida Coronel Theodolino Pereira de Araújo";
        [SerializeField] private string neighborhood = "Centro";
        [SerializeField] private RoadClassification classification = RoadClassification.ArterialDupla;

        [Header("Geometria e Perfil")]
        [SerializeField] private float roadWidthMeters = 14f;
        [SerializeField] private float sidewalkWidthMeters = 2.5f;
        [SerializeField] private int laneCount = 2;
        [SerializeField] private bool isOneWay = false;
        [SerializeField] private float speedLimitKmh = 50f;

        [Header("Pontos Geográficos Extremos (WGS84)")]
        [SerializeField] private double startLat = -18.649100;
        [SerializeField] private double startLon = -48.190300;
        [SerializeField] private double endLat = -18.647500;
        [SerializeField] private double endLon = -48.188200;

        [Header("Conectividade e Navegação")]
        [SerializeField] private List<TrafficWaypoint> associatedWaypoints = new List<TrafficWaypoint>();
        [SerializeField] private List<TrafficLane> associatedLanes = new List<TrafficLane>();
        [SerializeField] private bool hasPedestrianCrossing = true;

        public string RoadName => roadName;
        public string Neighborhood => neighborhood;
        public RoadClassification Classification => classification;
        public float RoadWidthMeters => roadWidthMeters;
        public float SidewalkWidthMeters => sidewalkWidthMeters;
        public int LaneCount => laneCount;
        public bool IsOneWay => isOneWay;
        public float SpeedLimitKmh => speedLimitKmh;
        public double StartLat => startLat;
        public double StartLon => startLon;
        public double EndLat => endLat;
        public double EndLon => endLon;
        public IReadOnlyList<TrafficWaypoint> AssociatedWaypoints => associatedWaypoints;
        public IReadOnlyList<TrafficLane> AssociatedLanes => associatedLanes;
        public bool HasPedestrianCrossing => hasPedestrianCrossing;

        public void Configure(string name, string neigh, RoadClassification cls, float width, int lanes, bool oneWay, float speed)
        {
            roadName = name;
            neighborhood = neigh;
            classification = cls;
            roadWidthMeters = width;
            laneCount = lanes;
            isOneWay = oneWay;
            speedLimitKmh = speed;
        }

        public void SetGeoCoordinates(double sLat, double sLon, double eLat, double eLon)
        {
            startLat = sLat;
            startLon = sLon;
            endLat = eLat;
            endLon = eLon;
        }
    }
}
