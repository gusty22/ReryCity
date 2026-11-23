using System;
using UnityEngine;

namespace ProjetoGrau2.Police
{
    public enum PoliceAIState
    {
        Patrol,             // Patrulhamento urbano preventivo, respeitando tráfego e limites
        Suspicious,         // Alerta prévio (infração menor, ruído ou passagem rápida)
        Investigating,      // Aproximação para identificação visual e verificação da moto
        Pursuit,            // Perseguição ostensiva ativa com sirenes ligadas e alta velocidade
        Intercepting,       // Manobra de corte de rota e bloqueio lateral tático à frente
        Arresting,          // Abordagem iminente e ordem de prisão (veículo cercado/parado)
        LostTarget,         // Contato visual perdido; varredura na Última Posição Conhecida (LKP)
        ReturningToPatrol   // Desistência da perseguição e retorno ao fluxo viário normal
    }

    /// <summary>
    /// Máquina de estados finita hierárquica para a IA policial (Viaturas e ROCAM).
    /// Controla transições táticas, histerese temporal e garante que emergências
    /// ou ordens de perseguição direta tenham prioridade de execução.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Police AI State Machine")]
    public class PoliceAIStateMachine : MonoBehaviour
    {
        [Header("Estado Policial Atual")]
        [SerializeField] private PoliceAIState currentState = PoliceAIState.Patrol;
        [SerializeField] private PoliceAIState previousState = PoliceAIState.Patrol;

        [Header("Histerese e Temporização")]
        [Tooltip("Tempo mínimo de permanência no estado para evitar flutuações rápidas (segundos).")]
        [Range(0.1f, 2.0f)] public float minStateDuration = 0.4f;

        private float _stateTimer = 0f;

        public event Action<PoliceAIState, PoliceAIState> OnPoliceStateChanged;

        public PoliceAIState CurrentState => currentState;
        public PoliceAIState PreviousState => previousState;
        public float TimeInCurrentState => _stateTimer;
        public bool CanTransition => _stateTimer >= minStateDuration;

        private void Awake()
        {
            _stateTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            _stateTimer += deltaTime;
        }

        /// <summary>
        /// Solicita transição de estado policial.
        /// Transições para Pursuit, Intercepting e Arresting ignoram a histerese.
        /// </summary>
        public bool RequestState(PoliceAIState newState, bool force = false)
        {
            if (newState == currentState) return false;

            bool isHighPriority = (newState == PoliceAIState.Pursuit ||
                                  newState == PoliceAIState.Intercepting ||
                                  newState == PoliceAIState.Arresting);

            if (!isHighPriority && !force && !CanTransition)
            {
                return false;
            }

            previousState = currentState;
            currentState = newState;
            _stateTimer = 0f;

            OnPoliceStateChanged?.Invoke(previousState, currentState);
            return true;
        }

        public void ResetStateMachine(PoliceAIState initialState = PoliceAIState.Patrol)
        {
            previousState = PoliceAIState.Patrol;
            currentState = initialState;
            _stateTimer = 0f;
        }
    }
}
