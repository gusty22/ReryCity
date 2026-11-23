using System;
using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Propulsion
{
    /// <summary>
    /// Módulo independente da injeção eletrônica programável Athlon.
    /// Gerencia limitador principal de RPM, controle de largada (Two-Step),
    /// histerese e emissão de eventos de estampidos no escapamento.
    /// </summary>
    public class AthlonECUController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private AthlonECUData data;

        [Header("Ponto de Saída do Escapamento")]
        [SerializeField] private Transform exhaustTipTransform;

        // Estados em tempo de execução
        private bool isCutting = false;
        private bool isTwoStepActive = false;
        private float lastPopTime = 0f;

        // Eventos públicos desacoplados (áudio/partículas subscrevem aqui)
        public event Action<bool> OnEcuCutStateChanged;
        public event Action<float, Vector3> OnBackfireTriggered; // volume, posição do escape

        public AthlonECUData Data => data;
        public bool IsCutting => isCutting;
        public bool IsTwoStepActive => isTwoStepActive;

        public void Initialize(AthlonECUData ecuData)
        {
            data = ecuData;
        }

        /// <summary>
        /// Atualização física da ECU executada a cada FixedUpdate.
        /// </summary>
        /// <param name="currentRpm">Rotação atual do virabrequim (RPM).</param>
        /// <param name="speedKmh">Velocidade atual da motocicleta (km/h).</param>
        /// <param name="throttle">Acelerador (0.0 a 1.0).</param>
        /// <param name="clutchEngagement">Engate da embreagem (0 a 1).</param>
        /// <returns>True se a ECU estiver impondo corte de torque no motor.</returns>
        public bool EvaluateCut(float currentRpm, float speedKmh, float throttle, float clutchEngagement)
        {
            if (data == null || !data.ecuActive)
            {
                if (isCutting)
                {
                    isCutting = false;
                    OnEcuCutStateChanged?.Invoke(false);
                }
                return false;
            }

            // 1. Determinação da condição de Two-Step (largada: parada ou embreagem acionada com acelerador alto)
            isTwoStepActive = data.twoStepActive && (speedKmh < 4f || clutchEngagement < 0.2f) && throttle > 0.75f;

            // 2. Limiar de RPM e histerese aplicáveis
            float targetCutRpm = isTwoStepActive ? data.twoStepRpm : data.mainCutRpm;
            float hysteresis = isTwoStepActive ? data.twoStepHysteresis : data.hysteresisRpm;

            // 3. Comparador com Histerese (Schmitt Trigger)
            bool previousCut = isCutting;

            if (!isCutting && currentRpm >= targetCutRpm)
            {
                isCutting = true;
            }
            else if (isCutting && currentRpm <= (targetCutRpm - hysteresis))
            {
                isCutting = false;
            }

            if (isCutting != previousCut)
            {
                OnEcuCutStateChanged?.Invoke(isCutting);
            }

            // 4. Geração de Estampidos (Pops & Bangs) no escapamento durante o corte
            if (isCutting && (data.cutMode == EcuCutMode.IgnitionCut || data.cutMode == EcuCutMode.Both))
            {
                if (Time.time - lastPopTime >= data.minTimeBetweenPops)
                {
                    if (UnityEngine.Random.value <= data.backfireProbability)
                    {
                        lastPopTime = Time.time;
                        Vector3 exhaustPos = exhaustTipTransform != null ? exhaustTipTransform.position : transform.position;
                        OnBackfireTriggered?.Invoke(data.popVolume, exhaustPos);
                    }
                }
            }

            return isCutting;
        }
    }
}
