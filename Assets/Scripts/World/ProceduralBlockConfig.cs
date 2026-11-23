using System;
using UnityEngine;

namespace ProjetoGrau2.World
{
    public enum UrbanBlockType
    {
        Residential,
        Commercial,
        Industrial,
        CivicCenter,
        Suburban
    }

    /// <summary>
    /// Especificação e parâmetros de geração de quarteirões urbanos procedurais (Procedural Block Config).
    /// Define dimensões de quadra, larguras de calçada e via, quantidade de lotes e gabarito predial.
    /// </summary>
    [CreateAssetMenu(fileName = "New_ProceduralBlockConfig", menuName = "Projeto Grau 2/World/Procedural Block Config")]
    public class ProceduralBlockConfig : ScriptableObject
    {
        [Header("Tipo e Classificação")]
        public UrbanBlockType blockType = UrbanBlockType.Residential;
        public string blockName = "Quarteirão Residencial Padrão";

        [Header("Geometria da Quadra")]
        [Tooltip("Dimensões do quarteirão em metros (X e Z).")]
        public Vector2 blockSize = new Vector2(80f, 80f);

        [Tooltip("Largura da faixa asfáltica circundante (metros).")]
        [Range(6f, 24f)] public float roadWidth = 14f;

        [Tooltip("Largura da calçada para pedestres (metros).")]
        [Range(1.5f, 6f)] public float sidewalkWidth = 3f;

        [Header("Lotes e Edificações")]
        [Range(1, 4)] public int lotCountX = 2;
        [Range(1, 4)] public int lotCountZ = 2;
        [Range(4f, 15f)] public float buildingMinHeight = 6f;
        [Range(10f, 60f)] public float buildingMaxHeight = 22f;

        [Header("Mobiliário Urbano")]
        public bool hasStreetLights = true;
        public bool hasTrees = true;
        public bool hasHydrants = false;
        public float streetLightSpacing = 20f;
    }
}
