using UnityEngine;

namespace ProjetoGrau2.Data
{
    public enum RiderEquipmentCategory
    {
        Helmet,       // Capacetes (San Marino, fechado, robocop, balaclava)
        Shirt,        // Camisetas (time de futebol, regatas, corta-vento)
        Pants,        // Bermudas jeans rasgadas, tactel, calças
        Footwear,     // Calçados (tênis esportivos, chinelo de dedo)
        Gloves,       // Luvas de manobras
        Accessories   // Acessórios (óculos espelhado, mochila bag de entrega)
    }

    /// <summary>
    /// Configuração e dados de vestimentas e equipamentos do piloto.
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewEquipmentItem", menuName = "Projeto Grau 2/Piloto/Equipment Item Data")]
    public class RiderEquipmentData : ScriptableObject
    {
        [Header("Identificação do Equipamento")]
        public string equipmentId = "equip_helmet_san_marino";
        public string equipmentName = "Capacete San Marino Preto Fosco";
        public RiderEquipmentCategory category = RiderEquipmentCategory.Helmet;
        public int priceCentavos = 18000; // R$ 180,00

        [Header("Assets Visuais")]
        public GameObject equipmentPrefab;
        public Material materialOverride;
        public Sprite icon;

        [Header("Configuração de Encaixe")]
        [Tooltip("Offset relativo de posicionamento no osso âncora (X, Y, Z).")]
        public Vector3 localOffset = Vector3.zero;
        public Vector3 localEulerRotation = Vector3.zero;
    }
}
