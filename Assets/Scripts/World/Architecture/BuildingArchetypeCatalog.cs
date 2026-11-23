using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    [System.Serializable]
    public struct BuildingArchetype
    {
        public string archetypeName;
        public BuildingStyleCategory category;
        public float minWidth;
        public float maxWidth;
        public float minDepth;
        public float maxDepth;
        public float floorHeight;
        public int minFloors;
        public int maxFloors;
        public Color typicalFacadeColor;
        public bool hasWallFencing;      // Muro frontal com portão
        public bool hasRoofTiles;        // Telhado cerâmico colonial
    }

    /// <summary>
    /// Catálogo central de arquétipos arquitetônicos da cidade de Araguari para RERY CITY.
    /// Define proporções, gabaritos de altura e detalhes construtivos regionais.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildingArchetypeCatalog_Default", menuName = "Rery City/Architecture/Building Archetype Catalog")]
    public class BuildingArchetypeCatalog : ScriptableObject
    {
        [Header("Arquétipos Urbanos")]
        [SerializeField] private List<BuildingArchetype> archetypes = new List<BuildingArchetype>();

        public IReadOnlyList<BuildingArchetype> Archetypes => archetypes;

        public BuildingArchetype GetArchetypeForCategory(BuildingStyleCategory cat)
        {
            for (int i = 0; i < archetypes.Count; i++)
            {
                if (archetypes[i].category == cat)
                {
                    return archetypes[i];
                }
            }

            // Fallback padrão
            return new BuildingArchetype
            {
                archetypeName = "PadraoMedioFallback",
                category = BuildingStyleCategory.CasaPadraoMedio,
                minWidth = 10f,
                maxWidth = 15f,
                minDepth = 15f,
                maxDepth = 25f,
                floorHeight = 3.0f,
                minFloors = 1,
                maxFloors = 2,
                typicalFacadeColor = new Color(0.9f, 0.85f, 0.75f),
                hasWallFencing = true,
                hasRoofTiles = true
            };
        }
    }
}
