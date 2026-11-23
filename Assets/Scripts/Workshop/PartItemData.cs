using System;
using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Workshop
{
    public enum PartCategory
    {
        Engine,        // Motores completos e kits de cilindro
        Exhaust,       // Escapamentos (estética, desempenho e ronco acústico)
        Transmission,  // Relação de coroa e pinhão (foco em torque no grau ou final)
        Suspension,    // Molas, bengalas e amortecedores (molejo)
        Brakes,        // Discos, pinças e pastilhas esportivas
        TiresWheels,   // Pneus de manobra e rodas
        Handlebars,    // Guidões oxxy, originais ou esportivos
        Bodywork,      // Carenagens, rabeta, protetor de motor
        Paint          // Pinturas e texturas personalizadas
    }

    /// <summary>
    /// Definição de uma peça ou acessório customizável na oficina.
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewPartItem", menuName = "Projeto Grau 2/Oficina/Part Item Data")]
    public class PartItemData : ScriptableObject
    {
        [Header("Identificação da Peça")]
        public string partId = "part_exhaust_dore";
        public string partName = "Escapamento Dore Esportivo";
        public PartCategory category = PartCategory.Exhaust;
        [TextArea(2, 4)] public string description = "Aumenta a potência em alta rotação e confere estampido estalado característico.";
        public int priceCentavos = 85000; // R$ 850,00

        [Header("Modificadores Mecânicos (Opcionais)")]
        [Tooltip("Substituição de configuração de motor (se for da categoria Engine).")]
        public EngineData engineOverride;

        [Tooltip("Substituição de perfil de áudio acústico (se for da categoria Exhaust).")]
        public MotorAudioProfile audioProfileOverride;

        [Tooltip("Relação final coroa/pinhão customizada (se for da categoria Transmission, ex: 48/14 = 3.428 para grau rápido).")]
        public float customFinalDriveRatio = 0f;

        [Tooltip("Multiplicador da força de frenagem (1.0 = original, 1.35 = freio superdimensionado).")]
        [Range(0.5f, 2.0f)] public float brakeForceMultiplier = 1.0f;

        [Tooltip("Multiplicador de aderência lateral dos pneus.")]
        [Range(0.5f, 2.0f)] public float tireGripMultiplier = 1.0f;

        [Tooltip("Massa adicional ou redução de peso gerada pela peça (kg).")]
        public float weightDeltaKg = -2.5f;

        [Header("Representação Visual")]
        public GameObject prefabVisual;
        public Sprite icon;
    }
}
