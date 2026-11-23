using UnityEngine;

namespace ProjetoGrau2.World.Geography
{
    /// <summary>
    /// Configuração do Marco Zero geográfico para projeção cartográfica no Unity.
    /// Define as coordenadas do ponto central da cidade de Araguari (MG).
    /// </summary>
    [CreateAssetMenu(fileName = "GeoOrigin_AraguariCentro", menuName = "Rery City/Geography/Geo Origin Config")]
    public class GeoOriginConfig : ScriptableObject
    {
        [Header("Ponto de Referência Cartográfico")]
        [Tooltip("Nome do marco geográfico (ex: Praça Manoel Bonito, Araguari).")]
        public string originName = "Praça Manoel Bonito, Centro, Araguari - MG";

        [Tooltip("Latitude em graus decimais (WGS84).")]
        public double latitude = -18.649100;

        [Tooltip("Longitude em graus decimais (WGS84).")]
        public double longitude = -48.190300;

        [Tooltip("Altitude acima do nível do mar em metros.")]
        public double altitudeMeters = 970.0;

        [Header("Calibração Espacial")]
        [Tooltip("Fator de escala métrica (1.0 = 1 unidade Unity equivale a 1 metro real).")]
        [Range(0.1f, 10.0f)]
        public double scaleFactor = 1.0;

        [Tooltip("Rotação de azimute para alinhamento da malha viária com eixos do motor (graus).")]
        [Range(-180.0f, 180.0f)]
        public double headingRotationDegrees = 0.0;
    }
}
