using System;
using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Propulsion
{
    /// <summary>
    /// Controlador físico da transmissão: relações de marcha, embreagem e transferência de torque à roda traseira.
    /// Opera em conjunto com o EngineController em FixedUpdate.
    /// </summary>
    public class TransmissionController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private TransmissionData data;

        [Header("Estado do Câmbio")]
        [Tooltip("0 = Neutro, 1 = 1ª Marcha, 2 = 2ª Marcha, etc.")]
        [SerializeField] private int currentGear = 1;

        // Estados em tempo de execução
        private float clutchEngagement = 1.0f; // 1 = engatada (acoplada), 0 = acionada (aberta)
        private bool isShifting = false;
        private float shiftTimer = 0f;
        private float wheelTorqueNm;
        private float tractiveForceN;

        // Eventos públicos
        public event Action<int> OnGearChanged;
        public event Action OnShiftStarted;
        public event Action OnShiftCompleted;

        public TransmissionData Data => data;
        public int CurrentGear => currentGear;
        public float ClutchEngagement => clutchEngagement;
        public bool IsShifting => isShifting;
        public float WheelTorqueNm => wheelTorqueNm;
        public float TractiveForceN => tractiveForceN;

        public void Initialize(TransmissionData transmissionData)
        {
            data = transmissionData;
        }

        public void ShiftUp()
        {
            if (data == null || isShifting) return;
            if (currentGear < data.TotalForwardGears)
            {
                StartShift(currentGear + 1);
            }
        }

        public void ShiftDown()
        {
            if (data == null || isShifting) return;
            if (currentGear > 0)
            {
                StartShift(currentGear - 1);
            }
        }

        public void SetGear(int targetGear)
        {
            if (data == null || isShifting) return;
            targetGear = Mathf.Clamp(targetGear, 0, data.TotalForwardGears);
            if (targetGear != currentGear)
            {
                StartShift(targetGear);
            }
        }

        private void StartShift(int targetGear)
        {
            isShifting = true;
            shiftTimer = data.shiftDurationSeconds;
            currentGear = targetGear;
            OnShiftStarted?.Invoke();
            OnGearChanged?.Invoke(currentGear);
        }

        /// <summary>
        /// Processa a dinâmica da transmissão em FixedUpdate.
        /// </summary>
        /// <param name="engineTorque">Torque líquido entregue pelo virabrequim (Nm).</param>
        /// <param name="wheelLinearSpeedMs">Velocidade linear da roda traseira (m/s).</param>
        /// <param name="wheelRadius">Raio dinâmico do pneu traseiro (m).</param>
        /// <param name="manualClutchInput">Entrada de embreagem manual (0 = solta/acoplada, 1 = puxada na manete).</param>
        /// <param name="deltaTime">Passo de tempo da física.</param>
        /// <returns>RPM equivalente que a roda impõe ao virabrequim.</returns>
        public float SimulateTransmission(float engineTorque, float wheelLinearSpeedMs, float wheelRadius, float manualClutchInput, float deltaTime)
        {
            if (data == null) return 0f;

            // 1. Processamento do temporizador de troca de marcha
            if (isShifting)
            {
                shiftTimer -= deltaTime;
                if (shiftTimer <= 0f)
                {
                    isShifting = false;
                    OnShiftCompleted?.Invoke();
                }
            }

            // 2. Cálculo do engate da embreagem
            float targetEngagement = 1.0f - Mathf.Clamp01(manualClutchInput);
            if (isShifting && !data.allowClutchlessShifting)
            {
                targetEngagement = 0f; // Desacopla temporariamente durante o engate da marcha
            }

            // Auto-embreagem preventiva para evitar afogamento da moto parada em 1ª marcha
            float speedKmh = Mathf.Abs(wheelLinearSpeedMs) * 3.6f;
            if (currentGear == 1 && speedKmh < 3f && manualClutchInput < 0.1f)
            {
                targetEngagement = Mathf.Clamp01(speedKmh / 3f);
            }

            clutchEngagement = Mathf.MoveTowards(clutchEngagement, targetEngagement, data.clutchEngagementRate * deltaTime);

            // 3. Relação total de redução
            float totalRatio = data.GetTotalGearRatio(currentGear);

            // 4. Cálculo do RPM que a roda impõe de volta ao virabrequim
            // omega_roda = v / r;  RPM_roda = omega_roda * (60 / 2pi);  RPM_virabrequim = RPM_roda * totalRatio
            float wheelAngularVel = Mathf.Abs(wheelLinearSpeedMs) / Mathf.Max(0.05f, wheelRadius);
            float wheelRpm = wheelAngularVel * (60f / (2f * Mathf.PI));
            float engineEquivalentRpm = wheelRpm * totalRatio;

            // 5. Transferência de torque à roda traseira
            if (currentGear != 0 && clutchEngagement > 0.01f)
            {
                wheelTorqueNm = engineTorque * totalRatio * data.mechanicalEfficiency * clutchEngagement;
                tractiveForceN = wheelTorqueNm / Mathf.Max(0.05f, wheelRadius);
            }
            else
            {
                wheelTorqueNm = 0f;
                tractiveForceN = 0f;
            }

            return engineEquivalentRpm;
        }
    }
}
