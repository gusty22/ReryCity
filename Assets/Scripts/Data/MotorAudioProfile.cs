using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Perfil de configuração acústica do motor e escapamento.
    /// Define parâmetros de modulação de pitch, volume e referências de clips.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMotorAudioProfile", menuName = "Projeto Grau 2/Áudio/Motor Audio Profile")]
    public class MotorAudioProfile : ScriptableObject
    {
        [Header("Clips de Áudio Fundamentais")]
        [Tooltip("Som do motor em marcha lenta constante.")]
        public AudioClip idleClip;

        [Tooltip("Som do motor em baixa/média rotação.")]
        public AudioClip lowRpmClip;

        [Tooltip("Som do motor em alta rotação (plena carga).")]
        public AudioClip highRpmClip;

        [Tooltip("Sons variados de estampidos e tiros no escapamento.")]
        public AudioClip[] backfirePops;

        [Tooltip("Som de engate / estalo mecânico da troca de marcha.")]
        public AudioClip gearShiftClip;

        [Header("Curvas de Modulação de Pitch")]
        [Tooltip("Pitch mínimo em marcha lenta.")]
        [Range(0.4f, 1.0f)] public float minPitch = 0.7f;

        [Tooltip("Pitch máximo no teto de giro.")]
        [Range(1.2f, 3.0f)] public float maxPitch = 2.2f;

        [Header("Controle de Volume")]
        [Tooltip("Volume da marcha lenta.")]
        [Range(0f, 1f)] public float idleVolume = 0.65f;

        [Tooltip("Volume sob aceleração plena.")]
        [Range(0.5f, 1.5f)] public float throttleVolumeBoost = 1.0f;

        [Tooltip("Atenuação do som sob freio-motor (desaceleração sem acelerador).")]
        [Range(0.1f, 1f)] public float decelerationVolumeFactor = 0.5f;

        /// <summary>
        /// Calcula o pitch de áudio correspondente a um dado RPM.
        /// </summary>
        public float CalculatePitch(float currentRpm, float minRpm, float maxRpm)
        {
            float t = Mathf.Clamp01((currentRpm - minRpm) / Mathf.Max(1f, maxRpm - minRpm));
            return Mathf.Lerp(minPitch, maxPitch, t);
        }
    }
}
