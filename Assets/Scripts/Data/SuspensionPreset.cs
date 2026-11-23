using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Configuração e parametrização física das molas e amortecedores (Molejo).
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewSuspensionPreset", menuName = "Projeto Grau 2/Física/Suspension Preset")]
    public class SuspensionPreset : ScriptableObject
    {
        [Header("Suspensão Dianteira (Garfo Telescópico)")]
        [Tooltip("Constante de rigidez da mola dianteira em N/m (k_front).")]
        [Range(8000f, 40000f)] public float frontSpringStiffness = 18000f;

        [Tooltip("Constante de amortecimento viscoso dianteiro em Ns/m (c_front).")]
        [Range(500f, 4000f)] public float frontDamping = 1400f;

        [Tooltip("Comprimento de repouso útil da suspensão dianteira em metros.")]
        [Range(0.2f, 0.7f)] public float frontRestLength = 0.45f;

        [Header("Suspensão Traseira (Balança / Monoamortecedor)")]
        [Tooltip("Constante de rigidez da mola traseira em N/m (k_rear).")]
        [Range(10000f, 55000f)] public float rearSpringStiffness = 24000f;

        [Tooltip("Constante de amortecimento viscoso traseiro em Ns/m (c_rear).")]
        [Range(600f, 5000f)] public float rearDamping = 1800f;

        [Tooltip("Comprimento de repouso útil da suspensão traseira em metros.")]
        [Range(0.2f, 0.6f)] public float rearRestLength = 0.38f;

        [Header("Pré-Carga e Altura Livre")]
        [Tooltip("Ajuste de altura livre do solo (offset de pré-carga em metros).")]
        [Range(-0.1f, 0.15f)] public float rideHeightOffset = 0f;
    }
}
