using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    [System.Serializable]
    public struct NeighborhoodTheme
    {
        public string neighborhoodName;
        public float buildingDensity;      // 0.0 a 1.0
        public float treeDensity;          // Quantidade média de árvores por 100m
        public string dominantSidewalkType;// Concreto, PedraPortuguesa, Mosaico
        public float commercialRatio;      // Proporção de comércio vs residencial (0.0 a 1.0)
        public float averageBuildingFloors;// Média de andares
        public Color primaryAmbientColor;  // Tonalidade de fachadas e ambientação
    }

    /// <summary>
    /// Catálogo central de identidades visuais e temáticas dos bairros de Araguari para RERY CITY.
    /// Define densidade construtiva, calçamento e perfil urbanístico de cada região urbana.
    /// </summary>
    [CreateAssetMenu(fileName = "NeighborhoodThemes_Default", menuName = "Rery City/Architecture/Neighborhood Themes Catalog")]
    public class NeighborhoodThemeCatalog : ScriptableObject
    {
        [Header("Temas Urbanos por Bairro")]
        [SerializeField] private List<NeighborhoodTheme> themes = new List<NeighborhoodTheme>();

        public IReadOnlyList<NeighborhoodTheme> Themes => themes;

        public NeighborhoodTheme GetThemeForNeighborhood(string neighborhoodName)
        {
            if (themes != null)
            {
                for (int i = 0; i < themes.Count; i++)
                {
                    if (string.Equals(themes[i].neighborhoodName, neighborhoodName, StringComparison.OrdinalIgnoreCase))
                    {
                        return themes[i];
                    }
                }
            }

            // Fallback genérico para bairros não mapeados
            return new NeighborhoodTheme
            {
                neighborhoodName = neighborhoodName ?? "Padrao",
                buildingDensity = 0.6f,
                treeDensity = 4.0f,
                dominantSidewalkType = "Concreto",
                commercialRatio = 0.2f,
                averageBuildingFloors = 1.5f,
                primaryAmbientColor = new Color(0.9f, 0.85f, 0.8f)
            };
        }
    }
}
