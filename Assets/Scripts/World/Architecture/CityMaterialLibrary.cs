using System;
using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    public enum CityMaterialType
    {
        AsfaltoVia,
        ConcretoMeioFio,
        CalcadaPedestre,
        FachadaResidencial,
        FachadaComercial,
        TelhadoColonial,
        VidroJanela,
        MetalEstrutural,
        VegetacaoArvores,
        SinalizacaoBranca,
        SinalizacaoAmarela
    }

    [System.Serializable]
    public struct MaterialDefinition
    {
        public CityMaterialType materialType;
        public Material sharedMaterial;
        public Color baseFallbackColor;
        public float smoothness;
        public bool enableGPUInstancing;
    }

    /// <summary>
    /// Biblioteca centralizada de materiais compartilhados para RERY CITY.
    /// Promove Static Batching e GPU Instancing no Universal Render Pipeline (URP),
    /// minimizando Draw Calls e alocações de memória gráfica em dispositivos Android.
    /// </summary>
    [CreateAssetMenu(fileName = "CityMaterialLibrary_Default", menuName = "Rery City/Architecture/City Material Library")]
    public class CityMaterialLibrary : ScriptableObject
    {
        [Header("Materiais Urbanos Compartilhados")]
        [SerializeField] private MaterialDefinition[] materialDefinitions;

        public MaterialDefinition[] MaterialDefinitions => materialDefinitions;

        public MaterialDefinition GetDefinition(CityMaterialType type)
        {
            if (materialDefinitions != null)
            {
                for (int i = 0; i < materialDefinitions.Length; i++)
                {
                    if (materialDefinitions[i].materialType == type)
                    {
                        return materialDefinitions[i];
                    }
                }
            }

            // Definição fallback
            return new MaterialDefinition
            {
                materialType = type,
                sharedMaterial = null,
                baseFallbackColor = Color.gray,
                smoothness = 0.2f,
                enableGPUInstancing = true
            };
        }
    }
}
