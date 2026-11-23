using System;
using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Configuração da transmissão mecânica: caixa de câmbio, embreagem e relação final coroa/pinhão.
    /// Segue a arquitetura Data-Driven (ADR-003).
    /// </summary>
    [CreateAssetMenu(fileName = "NewTransmissionData", menuName = "Projeto Grau 2/Propulsão/Transmission Data")]
    public class TransmissionData : ScriptableObject
    {
        [Header("Relações de Marcha (Câmbio)")]
        [Tooltip("Relação de redução de cada marcha para frente (1ª a N).")]
        public float[] forwardGearRatios = new float[]
        {
            2.769f, // 1ª Marcha
            1.882f, // 2ª Marcha
            1.400f, // 3ª Marcha
            1.130f, // 4ª Marcha
            0.960f  // 5ª Marcha
        };

        [Tooltip("Relação de redução da marcha à ré (se houver, ou 0 se desativada).")]
        public float reverseRatio = 0f;

        [Header("Reduções Primária e Final")]
        [Tooltip("Redução primária (engrenagem do virabrequim para a carcaça da embreagem).")]
        [Range(1.5f, 4.5f)] public float primaryReductionRatio = 3.087f;

        [Tooltip("Redução final (dentes da coroa traseira / dentes do pinhão, ex: 44/15 = 2.933).")]
        [Range(1.5f, 5.0f)] public float finalDriveRatio = 2.933f;

        [Tooltip("Eficiência mecânica do trem de força (perdas por atrito na corrente, rolamentos e caixa).")]
        [Range(0.7f, 0.98f)] public float mechanicalEfficiency = 0.92f;

        [Header("Embreagem e Troca de Marchas")]
        [Tooltip("Tempo em segundos que o mecanismo leva para desacoplar e engatar uma nova marcha.")]
        [Range(0.05f, 0.5f)] public float shiftDurationSeconds = 0.15f;

        [Tooltip("Permite troca rápida de marchas sem necessidade de acionar a embreagem manual (Quickshifter).")]
        public bool allowClutchlessShifting = false;

        [Tooltip("Rigidez do acoplamento dos discos de embreagem (velocidade de sincronização virabrequim-eixo).")]
        [Range(2f, 25f)] public float clutchEngagementRate = 12f;

        /// <summary>
        /// Quantidade de marchas para frente configuradas.
        /// </summary>
        public int TotalForwardGears => forwardGearRatios != null ? forwardGearRatios.Length : 0;

        /// <summary>
        /// Retorna a relação total de redução da marcha selecionada:
        /// R_total = R_primaria * R_marcha * R_final
        /// </summary>
        public float GetTotalGearRatio(int gearIndex)
        {
            if (gearIndex == 0) return 0f; // Neutro

            if (gearIndex > 0 && gearIndex <= TotalForwardGears)
            {
                return primaryReductionRatio * forwardGearRatios[gearIndex - 1] * finalDriveRatio;
            }

            if (gearIndex < 0 && reverseRatio > 0.01f)
            {
                return primaryReductionRatio * reverseRatio * finalDriveRatio;
            }

            return 0f;
        }
    }
}
