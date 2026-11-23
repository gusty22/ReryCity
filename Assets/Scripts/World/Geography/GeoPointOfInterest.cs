using UnityEngine;

namespace ProjetoGrau2.World.Geography
{
    public enum POICategory
    {
        PracaPublica,
        MarcoHistorico,
        OficinaMecanica,
        PostoCombustivel,
        ComercioCentral,
        ParqueReserva,
        ServicoPublico,
        PontoEncontroGrau
    }

    /// <summary>
    /// Representa um Ponto de Interesse (POI) real georreferenciado em RERY CITY (Araguari).
    /// </summary>
    [AddComponentMenu("Rery City/Geography/Geo Point Of Interest")]
    public class GeoPointOfInterest : MonoBehaviour
    {
        [Header("Identificação do Ponto")]
        [SerializeField] private string poiId = "POI_Centro_PracaManoelBonito";
        [SerializeField] private string poiName = "Praça Manoel Bonito";
        [SerializeField] private string neighborhood = "Centro";
        [SerializeField] private POICategory category = POICategory.PracaPublica;

        [Header("Coordenadas Geográficas")]
        [SerializeField] private double latitude = -18.649100;
        [SerializeField] private double longitude = -48.190300;
        [SerializeField] private double altitude = 970.0;

        [Header("Dimensões e Influência")]
        [SerializeField] private float radiusMeters = 30f;
        [SerializeField] private bool showGizmos = true;

        public string PoiId => poiId;
        public string PoiName => poiName;
        public string Neighborhood => neighborhood;
        public POICategory Category => category;
        public double Latitude => latitude;
        public double Longitude => longitude;
        public double Altitude => altitude;
        public float RadiusMeters => radiusMeters;

        /// <summary>
        /// Atualiza a posição no Unity com base na origem cartográfica fornecida.
        /// </summary>
        public void SyncPositionWithOrigin(GeoOriginConfig origin)
        {
            if (origin != null)
            {
                transform.position = GeoCoordinateConverter.GeoToWorld(latitude, longitude, altitude, origin);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Gizmos.color = GetCategoryColor(category);
            Gizmos.DrawWireSphere(transform.position, radiusMeters);
            Gizmos.DrawSphere(transform.position, 2f);
        }

        private Color GetCategoryColor(POICategory cat)
        {
            switch (cat)
            {
                case POICategory.PracaPublica: return Color.green;
                case POICategory.MarcoHistorico: return Color.yellow;
                case POICategory.OficinaMecanica: return new Color(1f, 0.5f, 0f);
                case POICategory.PostoCombustivel: return Color.red;
                case POICategory.PontoEncontroGrau: return Color.cyan;
                case POICategory.ParqueReserva: return new Color(0.1f, 0.8f, 0.3f);
                default: return Color.white;
            }
        }
    }
}
