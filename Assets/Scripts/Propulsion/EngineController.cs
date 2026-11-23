using System;
using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Propulsion
{
    public enum EngineState
    {
        Off,
        Starting,
        Running
    }

    /// <summary>
    /// Controlador físico da termodinâmica e cinemática rotacional do motor de combustão interna.
    /// Calcula RPM, torque líquido no virabrequim, potência em CV e resposta do acelerador a 60 Hz.
    /// </summary>
    public class EngineController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private EngineData data;

        [Header("Estado Operacional")]
        [SerializeField] private EngineState state = EngineState.Running;
        [SerializeField] private bool autoStartOnPlay = true;

        // Estados em tempo de execução
        private float currentRpm;
        private float effectiveThrottle;
        private float currentTorqueNm;
        private float currentPowerCv;
        private bool isCutActive;

        // Eventos públicos de notificação
        public event Action<float> OnRpmChanged;
        public event Action<bool> OnCutStateChanged;
        public event Action OnEngineStarted;
        public event Action OnEngineStopped;

        public EngineData Data => data;
        public EngineState State => state;
        public float CurrentRpm => currentRpm;
        public float EffectiveThrottle => effectiveThrottle;
        public float CurrentTorqueNm => currentTorqueNm;
        public float CurrentPowerCv => currentPowerCv;
        public bool IsCutActive => isCutActive;

        public void Initialize(EngineData engineData)
        {
            data = engineData;
            if (data != null && state == EngineState.Running)
            {
                currentRpm = data.idleRpm;
            }
        }

        private void Awake()
        {
            if (autoStartOnPlay)
            {
                StartEngine();
            }
        }

        public void StartEngine()
        {
            state = EngineState.Running;
            if (data != null)
            {
                currentRpm = data.idleRpm;
            }
            OnEngineStarted?.Invoke();
        }

        public void StopEngine()
        {
            state = EngineState.Off;
            currentRpm = 0f;
            currentTorqueNm = 0f;
            currentPowerCv = 0f;
            effectiveThrottle = 0f;
            OnEngineStopped?.Invoke();
        }

        /// <summary>
        /// Atualização física do motor chamada em FixedUpdate.
        /// </summary>
        /// <param name="rawThrottle">Entrada do acelerador do jogador (0.0 a 1.0).</param>
        /// <param name="wheelRpmEquivalent">RPM que a roda traseira está impondo ao motor via transmissão.</param>
        /// <param name="clutchEngagement">Grau de acoplamento da embreagem (0 = desacoplado/patinando, 1 = acoplado 100%).</param>
        /// <param name="ecuExternalCut">Sinal externo de corte emitido pela ECU Athlon.</param>
        /// <param name="deltaTime">Passo de tempo da física (FixedDeltaTime).</param>
        public void SimulatePhysics(float rawThrottle, float wheelRpmEquivalent, float clutchEngagement, bool ecuExternalCut, float deltaTime)
        {
            if (data == null || state != EngineState.Running)
            {
                currentRpm = Mathf.MoveTowards(currentRpm, 0f, 1500f * deltaTime);
                currentTorqueNm = 0f;
                currentPowerCv = 0f;
                return;
            }

            // 1. Rampa progressiva do acelerador (abertura e fechamento da borboleta)
            float rampSpeed = (rawThrottle > effectiveThrottle) ? data.throttleRampOpenSpeed : data.throttleRampCloseSpeed;
            effectiveThrottle = Mathf.MoveTowards(effectiveThrottle, Mathf.Clamp01(rawThrottle), rampSpeed * deltaTime);

            // 2. Verificação de corte (limitador interno de segurança ou corte externo da ECU Athlon)
            bool internalCut = currentRpm >= data.defaultLimiterRpm;
            bool newCutState = internalCut || ecuExternalCut;

            if (newCutState != isCutActive)
            {
                isCutActive = newCutState;
                OnCutStateChanged?.Invoke(isCutActive);
            }

            // 3. Torque de combustão bruto
            float grossTorque = isCutActive ? 0f : (effectiveThrottle * data.EvaluateTorque(currentRpm));

            // 4. Atrito interno e Freio-motor sob desaceleração
            float frictionTorque = data.internalFriction * (currentRpm / 1000f);
            if (effectiveThrottle < 0.05f && currentRpm > data.idleRpm)
            {
                frictionTorque += (currentRpm - data.idleRpm) * 0.003f * data.engineBrakingFactor;
            }

            // Torque líquido produzido no virabrequim
            currentTorqueNm = grossTorque - frictionTorque;

            // 5. Integração da Rotação (RPM)
            // Se desacoplado (ponto morto ou embreagem acionada):
            float netAngularAccel = (currentTorqueNm / Mathf.Max(0.001f, data.flywheelInertia)) * (60f / (2f * Mathf.PI));
            float freeRpm = currentRpm + netAngularAccel * deltaTime;

            // Marcha lenta atuador automático (controlador PID simplificado de marcha lenta)
            if (freeRpm < data.idleRpm)
            {
                freeRpm = Mathf.Lerp(freeRpm, data.idleRpm, 15f * deltaTime);
            }

            // Acoplamento mecânico com a transmissão/roda
            if (clutchEngagement > 0.01f && wheelRpmEquivalent > 50f)
            {
                // O RPM converge para a velocidade da roda proporcional ao acoplamento dos discos
                float targetCoupledRpm = Mathf.Max(data.idleRpm, wheelRpmEquivalent);
                currentRpm = Mathf.Lerp(freeRpm, targetCoupledRpm, clutchEngagement);
            }
            else
            {
                currentRpm = freeRpm;
            }

            // Clamping mecânico aos limites do motor
            currentRpm = Mathf.Clamp(currentRpm, 0f, data.maxRpm);

            // 6. Cálculo da Potência em Cavalos-Vapor (CV)
            currentPowerCv = Mathf.Max(0f, (currentTorqueNm * currentRpm) / 7023.5f);

            OnRpmChanged?.Invoke(currentRpm);
        }
    }
}
