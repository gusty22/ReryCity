using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Configuração e parametrização física do motor de combustão interna.
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewEngineData", menuName = "Projeto Grau 2/Propulsão/Engine Data")]
    public class EngineData : ScriptableObject
    {
        [Header("Identificação do Motor")]
        public string engineName = "Monocilíndrico 162.7cc OHC";
        public float displacementCc = 162.7f;

        [Header("Limites Operacionais de Rotação (RPM)")]
        [Tooltip("Rotação de marcha lenta contínua (estabilizada).")]
        [Range(800f, 2500f)] public float idleRpm = 1350f;

        [Tooltip("Rotação máxima suportada pelo conjunto de válvulas antes de flutuação mecânica.")]
        [Range(8000f, 16000f)] public float maxRpm = 11000f;

        [Tooltip("Limitador de giro padrão de fábrica.")]
        [Range(7000f, 15000f)] public float defaultLimiterRpm = 9800f;

        [Header("Curva de Torque Dinamométrico (RPM -> Nm)")]
        [Tooltip("Curva mapeando RPM real para torque líquido no virabrequim (Nm).")]
        public AnimationCurve torqueCurve = new AnimationCurve(
            new Keyframe(1000f, 6.0f),
            new Keyframe(3000f, 10.5f),
            new Keyframe(5500f, 13.8f),
            new Keyframe(7000f, 15.2f),
            new Keyframe(8500f, 14.1f),
            new Keyframe(9800f, 11.0f),
            new Keyframe(11000f, 4.0f)
        );

        [Header("Dinâmica do Volante e Resposta à Carga")]
        [Tooltip("Momento de inércia rotacional do virabrequim/volante (kg*m^2). Menor valor = sobe giro mais rápido no neutro.")]
        [Range(0.01f, 0.3f)] public float flywheelInertia = 0.045f;

        [Tooltip("Atrito interno do motor (resistência viscosa que reduz o RPM ao soltar o acelerador).")]
        [Range(0.5f, 5.0f)] public float internalFriction = 1.8f;

        [Tooltip("Fator multiplicador do freio-motor sob compressão quando em marcha engatada.")]
        [Range(0.5f, 4.0f)] public float engineBrakingFactor = 2.2f;

        [Header("Sensibilidade do Acelerador")]
        [Tooltip("Velocidade com que a borboleta do acelerador abre (resposta ao comando do jogador).")]
        [Range(5f, 35f)] public float throttleRampOpenSpeed = 18f;

        [Tooltip("Velocidade com que a borboleta fecha ao soltar o acelerador.")]
        [Range(5f, 35f)] public float throttleRampCloseSpeed = 22f;

        /// <summary>
        /// Avalia o torque do motor em Newton-metros para uma dada rotação.
        /// </summary>
        public float EvaluateTorque(float rpm)
        {
            return Mathf.Max(0f, torqueCurve.Evaluate(rpm));
        }

        /// <summary>
        /// Calcula a potência estimada em Cavalos-Vapor (CV) no virabrequim:
        /// Potencia(CV) = (Torque_Nm * RPM) / 7023.5
        /// </summary>
        public float CalculatePowerCv(float rpm)
        {
            float torque = EvaluateTorque(rpm);
            return (torque * rpm) / 7023.5f;
        }
    }
}
