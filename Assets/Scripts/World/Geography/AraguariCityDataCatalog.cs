using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Geography
{
    [System.Serializable]
    public struct NeighborhoodInfo
    {
        public string neighborhoodName;
        public string zoneType; // Comercial, Residencial, Historico, Misto, Industrial
        public double centerLatitude;
        public double centerLongitude;
        public float approximateRadiusMeters;
    }

    [System.Serializable]
    public struct RealRoadDefinition
    {
        public string roadName;
        public string neighborhood;
        public RoadClassification classification;
        public float roadWidth;
        public int laneCount;
        public bool isOneWay;
        public float speedLimit;
        public double startLat;
        public double startLon;
        public double endLat;
        public double endLon;
    }

    [System.Serializable]
    public struct RealPOIDefinition
    {
        public string poiId;
        public string poiName;
        public string neighborhood;
        public POICategory category;
        public double latitude;
        public double longitude;
        public double altitude;
        public float radiusMeters;
    }

    /// <summary>
    /// Catálogo central de dados cartográficos e estruturais da cidade real de Araguari (MG) para RERY CITY.
    /// Registra fontes cartográficas (OpenStreetMap / IBGE), bairros, vias principais e pontos notáveis.
    /// </summary>
    [CreateAssetMenu(fileName = "AraguariCityCatalog_Default", menuName = "Rery City/Geography/City Data Catalog")]
    public class AraguariCityDataCatalog : ScriptableObject
    {
        [Header("Metadados Cartográficos e Licenciamento")]
        [SerializeField] private string cityName = "Araguari";
        [SerializeField] private string stateAbbreviation = "MG";
        [SerializeField] private string cartographySource = "OpenStreetMap contributors (ODbL) / IBGE Malha Municipal";
        [SerializeField] private string licenseNotice = "Contém dados cartográficos públicos sob Open Database License (ODbL).";

        [Header("Bairros Urbanos Mapeados")]
        [SerializeField] private List<NeighborhoodInfo> neighborhoods = new List<NeighborhoodInfo>();

        [Header("Malha de Vias Reais")]
        [SerializeField] private List<RealRoadDefinition> roadDefinitions = new List<RealRoadDefinition>();

        [Header("Pontos de Interesse Notáveis (POIs)")]
        [SerializeField] private List<RealPOIDefinition> poiDefinitions = new List<RealPOIDefinition>();

        public string CityName => cityName;
        public string StateAbbreviation => stateAbbreviation;
        public string CartographySource => cartographySource;
        public string LicenseNotice => licenseNotice;
        public IReadOnlyList<NeighborhoodInfo> Neighborhoods => neighborhoods;
        public IReadOnlyList<RealRoadDefinition> RoadDefinitions => roadDefinitions;
        public IReadOnlyList<RealPOIDefinition> PoiDefinitions => poiDefinitions;
    }
}
