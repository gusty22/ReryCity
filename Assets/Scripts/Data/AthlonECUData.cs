using UnityEngine;

namespace ProjetoGrau2.Data
{
    public enum EcuCutMode
    {
        IgnitionCut, // Corta centelha (gera estampidos com combustível quente no escape)
        FuelCut,     // Corta injeção (corte suave, sem tiros)
        Both         // Corta ignição e injeção (corte seco rápido)
    }

    /// <summary>
    /// Configuração e parâmetros da injeção eletrônica programável Athlon.
    /// Modela limitadores de giro, two-step e cadência de estampidos.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAthlonECUData", menuName = "Projeto Grau 2/Propulsão/Athlon ECU Data")]
    public class AthlonECUData : ScriptableObject
    {
        [Header("Estado Geral")]
        public bool ecuActive = true;

        [Header("Limitador Principal (Hard Cut)")]
        [Tooltip("Rotação limite onde o corte é acionado.")]
        [Range(6000f, 15000f)] public float mainCutRpm = 10200f;

        [Tooltip("Histerese em RPM: faixa que o motor deve descer antes de religar a centelha.")]
        [Range(50f, 800f)] public float hysteresisRpm = 200f;

        [Tooltip("Tipo de intervenção de corte.")]
        public EcuCutMode cutMode = EcuCutMode.IgnitionCut;

        [Header("Controle de Largada (Two-Step)")]
        [Tooltip("Ativa limitador secundário de largada.")]
        public bool twoStepActive = true;

        [Tooltip("RPM de retenção do two-step (quando moto estiver parada e acelerador a fundo).")]
        [Range(3000f, 8500f)] public float twoStepRpm = 5200f;

        [Tooltip("Histerese específica do two-step.")]
        [Range(50f, 400f)] public float twoStepHysteresis = 120f;

        [Header("Estampidos no Escapamento (Pops & Bangs)")]
        [Tooltip("Probabilidade de disparo de estampido acústico/visual por ciclo de corte em corte de ignição.")]
        [Range(0f, 1f)] public float backfireProbability = 0.65f;

        [Tooltip("Tempo mínimo em segundos entre estampidos consecutivos (evita sobreposição sonora).")]
        [Range(0.02f, 0.2f)] public float minTimeBetweenPops = 0.05f;

        [Tooltip("Intensidade sonora relativa do estampido.")]
        [Range(0.5f, 2.0f)] public float popVolume = 1.0f;
    }
}
