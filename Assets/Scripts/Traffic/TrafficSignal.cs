using System;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    public enum TrafficSignalState
    {
        Red,
        Green,
        Yellow,
        AllRed // Intervalo de limpeza/esvaziamento do cruzamento antes de abrir o fluxo oposto
    }

    /// <summary>
    /// Controlador de semáforo urbano com ciclo de 4 fases (Verde, Amarelo, Vermelho e Limpeza All-Red).
    /// Suporta temporizações configuráveis, agrupamento por fases de cruzamento e eventos de transição.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Traffic/Traffic Signal")]
    public class TrafficSignal : MonoBehaviour
    {
        [Header("Identificação do Semáforo")]
        public string signalId = "signal_av_principal_sul";
        [Tooltip("Fase ou grupo deste semáforo no cruzamento (ex: Fase 0 = Norte-Sul, Fase 1 = Leste-Oeste).")]
        public int phaseGroup = 0;

        [Header("Temporização dos Ciclos (Segundos)")]
        [Range(5f, 60f)] public float greenDuration = 20f;
        [Range(2f, 8f)] public float yellowDuration = 3.5f;
        [Range(5f, 60f)] public float redDuration = 24f;
        [Range(1f, 5f)] public float allRedDuration = 2f;

        [Header("Estado Inicial")]
        public TrafficSignalState initialState = TrafficSignalState.Red;
        public float initialTimeOffset = 0f;

        [Header("Estado Atual (Runtime)")]
        [SerializeField] private TrafficSignalState currentState = TrafficSignalState.Red;
        [SerializeField] private float stateTimer = 0f;

        [Header("Lâmpadas Visuais (Opcional - Renderers/Luzes)")]
        [SerializeField] private Light greenLight;
        [SerializeField] private Light yellowLight;
        [SerializeField] private Light redLight;

        // Eventos
        public event Action<TrafficSignal, TrafficSignalState> OnStateChanged;

        public TrafficSignalState CurrentState => currentState;
        public float StateTimer => stateTimer;
        public bool IsGreen => currentState == TrafficSignalState.Green;
        public bool IsYellow => currentState == TrafficSignalState.Yellow;
        public bool IsRed => currentState == TrafficSignalState.Red || currentState == TrafficSignalState.AllRed;

        public float RemainingTimeInState
        {
            get
            {
                float target = currentState switch
                {
                    TrafficSignalState.Green => greenDuration,
                    TrafficSignalState.Yellow => yellowDuration,
                    TrafficSignalState.Red => redDuration,
                    TrafficSignalState.AllRed => allRedDuration,
                    _ => 0f
                };
                return Mathf.Max(0f, target - stateTimer);
            }
        }

        private void Awake()
        {
            currentState = initialState;
            stateTimer = initialTimeOffset;
            UpdateVisualLights();
        }

        private void Update()
        {
            TickSignal(Time.deltaTime);
        }

        /// <summary>
        /// Avança o relógio do semáforo por deltaTempo.
        /// </summary>
        public void TickSignal(float deltaTime)
        {
            stateTimer += deltaTime;

            switch (currentState)
            {
                case TrafficSignalState.Green:
                    if (stateTimer >= greenDuration)
                    {
                        SetState(TrafficSignalState.Yellow);
                    }
                    break;

                case TrafficSignalState.Yellow:
                    if (stateTimer >= yellowDuration)
                    {
                        SetState(TrafficSignalState.AllRed);
                    }
                    break;

                case TrafficSignalState.AllRed:
                    if (stateTimer >= allRedDuration)
                    {
                        SetState(TrafficSignalState.Red);
                    }
                    break;

                case TrafficSignalState.Red:
                    if (stateTimer >= redDuration)
                    {
                        SetState(TrafficSignalState.Green);
                    }
                    break;
            }
        }

        /// <summary>
        /// Força a transição de estado imediata (gerenciado centralmente pelo cruzamento ou manual).
        /// </summary>
        public void SetState(TrafficSignalState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            stateTimer = 0f;
            UpdateVisualLights();
            OnStateChanged?.Invoke(this, currentState);
        }

        private void UpdateVisualLights()
        {
            if (greenLight != null) greenLight.enabled = (currentState == TrafficSignalState.Green);
            if (yellowLight != null) yellowLight.enabled = (currentState == TrafficSignalState.Yellow);
            if (redLight != null) redLight.enabled = (currentState == TrafficSignalState.Red || currentState == TrafficSignalState.AllRed);
        }

        private void OnDrawGizmos()
        {
            Vector3 pos = transform.position;
            Color gizmoColor = currentState switch
            {
                TrafficSignalState.Green => Color.green,
                TrafficSignalState.Yellow => Color.yellow,
                TrafficSignalState.Red => Color.red,
                TrafficSignalState.AllRed => new Color(0.8f, 0f, 0f),
                _ => Color.white
            };

            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(pos, 0.45f);

            // Haste do semáforo
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(pos, pos - Vector3.up * 2.5f);
        }
    }
}
