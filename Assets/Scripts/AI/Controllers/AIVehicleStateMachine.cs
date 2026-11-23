using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    public enum AIVehicleState
    {
        Idle,                   // Parado (estacionado ou aguardando spawn/ativação)
        Cruise,                 // Velocidade de cruzeiro normal seguindo a faixa
        FollowVehicle,          // Seguindo veículo mais lento à frente mantendo distância segura
        Brake,                  // Desacelerando gradualmente (curva, aproximação de cruzamento ou lentidão)
        StopAtSignal,           // Retido na linha de retenção por semáforo vermelho ou amarelo tardio
        IntersectionWaiting,    // Aguardando liberação de cruzamento prioritário, stop sign ou rotatória
        LaneChanging,           // Executando manobra de transição lateral de faixa
        Overtaking,             // Manobra de ultrapassagem pela faixa da esquerda
        AvoidObstacle,          // Desvio seguro de obstáculo estático ou veículo imobilizado
        EmergencyStop           // Frenagem brusca de emergência (corte repentino de moto ou colisão iminente)
    }

    /// <summary>
    /// Máquina de estados finitos hierárquica para veículos NPC.
    /// Governa decisões de tráfego, histerese temporal e transições seguras sem oscilações bruscas.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle State Machine")]
    public class AIVehicleStateMachine : MonoBehaviour
    {
        [Header("Estado Atual")]
        [SerializeField] private AIVehicleState currentState = AIVehicleState.Cruise;
        [SerializeField] private AIVehicleState previousState = AIVehicleState.Idle;

        [Header("Histerese e Temporizadores")]
        [Tooltip("Tempo mínimo de permanência no estado atual antes de permitir nova transição (segundos).")]
        [Range(0.1f, 2.0f)] public float minStateDuration = 0.35f;

        private float stateTimer = 0f;
        private float laneChangeCooldownTimer = 0f;

        public event Action<AIVehicleState, AIVehicleState> OnStateChanged;

        public AIVehicleState CurrentState => currentState;
        public AIVehicleState PreviousState => previousState;
        public float TimeInCurrentState => stateTimer;
        public bool CanChangeState => stateTimer >= minStateDuration;
        public bool CanInitiateLaneChange => laneChangeCooldownTimer <= 0f;

        private void Awake()
        {
            stateTimer = 0f;
            laneChangeCooldownTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            stateTimer += deltaTime;
            if (laneChangeCooldownTimer > 0f)
            {
                laneChangeCooldownTimer -= deltaTime;
            }
        }

        /// <summary>
        /// Solicita transição de estado. Respeita histerese para estados normais,
        /// mas permite interrupção imediata em casos de emergência (EmergencyStop).
        /// </summary>
        public bool RequestState(AIVehicleState newState, bool force = false)
        {
            if (newState == currentState) return false;

            // Emergências furam a histerese
            bool isEmergency = (newState == AIVehicleState.EmergencyStop);

            if (!isEmergency && !force && !CanChangeState)
            {
                return false;
            }

            previousState = currentState;
            currentState = newState;
            stateTimer = 0f;

            if (newState == AIVehicleState.LaneChanging || newState == AIVehicleState.Overtaking)
            {
                laneChangeCooldownTimer = 4.0f; // Cooldown mínimo entre manobras
            }

            OnStateChanged?.Invoke(previousState, currentState);
            return true;
        }

        /// <summary>
        /// Reseta a máquina de estados para reinserção via Object Pool.
        /// </summary>
        public void ResetStateMachine(AIVehicleState startState = AIVehicleState.Cruise)
        {
            previousState = AIVehicleState.Idle;
            currentState = startState;
            stateTimer = 0f;
            laneChangeCooldownTimer = 0f;
        }
    }
}
