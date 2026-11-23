using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Configuração e parametrização física, ergonômica e postural do piloto humanoide.
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewRiderData", menuName = "Projeto Grau 2/Piloto/Rider Data")]
    public class RiderData : ScriptableObject
    {
        [Header("Identificação e Biometria")]
        public string riderName = "Piloto Padrão Grau";
        [Range(45f, 120f)] public float riderWeightKg = 72f;
        [Range(1.5f, 2.1f)] public float riderHeightMeters = 1.78f;

        [Header("Postura e Ergonomia de Pilotagem")]
        [Tooltip("Ângulo de inclinação padrão do tronco para frente em pilotagem normal (graus).")]
        [Range(0f, 45f)] public float spineForwardLeanAngle = 18f;

        [Tooltip("Projeção do tronco para trás durante o grau estabilizado (graus).")]
        [Range(0f, 50f)] public float wheelieSpineBackLeanAngle = 28f;

        [Tooltip("Projeção do tronco para frente sob aceleração brusca (reação à inércia).")]
        [Range(0f, 30f)] public float accelerationTuckAngle = 14f;

        [Tooltip("Projeção do corpo para trás sob frenagem forte.")]
        [Range(0f, 25f)] public float brakingPushBackAngle = 12f;

        [Header("Cinemática de Curva e Olhar")]
        [Tooltip("Grau de inclinação da cabeça e tronco em direção ao interior da curva.")]
        [Range(0f, 1.5f)] public float leanInCurveFactor = 0.85f;

        [Tooltip("Velocidade de rotação da cabeça para olhar na direção da curva ou da trajetória.")]
        [Range(2f, 20f)] public float headLookSpeed = 8f;

        [Header("Suavização e Inércia Corporal")]
        [Tooltip("Fator de amortecimento da movimentação postural do tronco.")]
        [Range(2f, 25f)] public float postureDamping = 10f;
    }
}
