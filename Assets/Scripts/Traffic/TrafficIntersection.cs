using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    public enum IntersectionPriorityType
    {
        TrafficLightControlled, // Controlado por semáforos sincronizados
        PriorityRoad,           // Via preferencial (faixas secundárias devem dar a preferência)
        AllWayStop,             // Parada obrigatória para todas as aproximações (Stop Sign)
        Roundabout              // Rotatória (prioridade de quem já circula)
    }

    /// <summary>
    /// Gerenciador de cruzamento viário urbano.
    /// Controla faixas de aproximação, saídas, manobras de virada (conversões), semáforos e regras de prioridade.
    /// Gerencia ocupação de veículos para prevenção ativa de bloqueios (deadlocks) e colisões internas.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Traffic/Traffic Intersection")]
    public class TrafficIntersection : MonoBehaviour
    {
        [Header("Identificação do Cruzamento")]
        public string intersectionId = "cross_centro_01";
        public IntersectionPriorityType priorityType = IntersectionPriorityType.TrafficLightControlled;

        [Header("Área de Conflito")]
        [Tooltip("Raio de abrangência do cruzamento em metros.")]
        [Range(5f, 50f)] public float conflictRadius = 15f;

        [Header("Faixas do Cruzamento")]
        [Tooltip("Faixas de rolamento que entram no cruzamento.")]
        [SerializeField] private List<TrafficLane> entryLanes = new List<TrafficLane>();

        [Tooltip("Faixas de rolamento que saem do cruzamento.")]
        [SerializeField] private List<TrafficLane> exitLanes = new List<TrafficLane>();

        [Tooltip("Faixas preferenciais (usado no modo PriorityRoad).")]
        [SerializeField] private List<TrafficLane> priorityLanes = new List<TrafficLane>();

        [Header("Semáforos Associados")]
        [Tooltip("Semáforos que governam este cruzamento.")]
        [SerializeField] private List<TrafficSignal> trafficSignals = new List<TrafficSignal>();

        [Header("Controle de Fases de Semáforo")]
        [Tooltip("Tempo de ciclo total da Fase A (Norte-Sul).")]
        public float phaseADuration = 25f;
        [Tooltip("Tempo de ciclo total da Fase B (Leste-Oeste).")]
        public float phaseBDuration = 20f;
        [Tooltip("Tempo de amarelo.")]
        public float yellowDuration = 3.5f;
        [Tooltip("Tempo de segurança (todos vermelhos).")]
        public float allRedDuration = 2f;

        // Estado interno do coordenador de fases
        [SerializeField] private int activePhaseIndex = 0;
        private float phaseTimer = 0f;

        // Rastreamento de ocupação em tempo real
        private readonly HashSet<Transform> occupyingVehicles = new HashSet<Transform>();

        public IReadOnlyList<TrafficLane> EntryLanes => entryLanes;
        public IReadOnlyList<TrafficLane> ExitLanes => exitLanes;
        public IReadOnlyList<TrafficSignal> TrafficSignals => trafficSignals;
        public int OccupyingVehicleCount => occupyingVehicles.Count;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            if (priorityType == IntersectionPriorityType.TrafficLightControlled && trafficSignals.Count > 0)
            {
                InitializeSignalCycle();
            }
        }

        private void Update()
        {
            if (priorityType == IntersectionPriorityType.TrafficLightControlled && trafficSignals.Count > 0)
            {
                UpdateCoordinatedSignals(Time.deltaTime);
            }
        }

        /// <summary>
        /// Inicializa as fases semafóricas coordenadas.
        /// </summary>
        public void InitializeSignalCycle()
        {
            activePhaseIndex = 0;
            phaseTimer = 0f;
            ApplySignalPhases();
        }

        private void UpdateCoordinatedSignals(float dt)
        {
            phaseTimer += dt;
            float currentPhaseDuration = (activePhaseIndex == 0) ? phaseADuration : phaseBDuration;

            if (phaseTimer >= currentPhaseDuration)
            {
                // Alternar fase
                activePhaseIndex = (activePhaseIndex == 0) ? 1 : 0;
                phaseTimer = 0f;
                ApplySignalPhases();
            }
        }

        private void ApplySignalPhases()
        {
            foreach (var sig in trafficSignals)
            {
                if (sig == null) continue;

                if (sig.phaseGroup == activePhaseIndex)
                {
                    sig.SetState(TrafficSignalState.Green);
                }
                else
                {
                    sig.SetState(TrafficSignalState.Red);
                }
            }
        }

        /// <summary>
        /// Avalia se um veículo pode ingressar no cruzamento respeitando semáforo, prioridade e bloqueio.
        /// </summary>
        public bool CanEnterIntersection(Transform vehicle, TrafficLane lane)
        {
            if (vehicle == null) return false;

            switch (priorityType)
            {
                case IntersectionPriorityType.TrafficLightControlled:
                    // Localiza o semáforo que governa esta faixa
                    TrafficSignal signal = GetSignalForLane(lane);
                    if (signal != null)
                    {
                        if (signal.IsRed) return false;
                        if (signal.IsYellow && occupyingVehicles.Count > 0) return false;
                    }
                    break;

                case IntersectionPriorityType.PriorityRoad:
                    // Se a faixa não for prioritária e houver veículo na área de conflito, deve dar preferência
                    if (!priorityLanes.Contains(lane) && occupyingVehicles.Count > 0)
                    {
                        return false;
                    }
                    break;

                case IntersectionPriorityType.AllWayStop:
                    // Se houver veículo no miolo do cruzamento, aguarda
                    if (occupyingVehicles.Count > 0)
                    {
                        return false;
                    }
                    break;

                case IntersectionPriorityType.Roundabout:
                    // Dá preferência aos veículos já dentro do anel giratório
                    if (occupyingVehicles.Count > 0)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Registra a entrada de um veículo no núcleo de conflito do cruzamento.
        /// </summary>
        public void RegisterVehicleEntry(Transform vehicle)
        {
            if (vehicle != null && !occupyingVehicles.Contains(vehicle))
            {
                occupyingVehicles.Add(vehicle);
            }
        }

        /// <summary>
        /// Registra a saída e liberação da área de conflito.
        /// </summary>
        public void RegisterVehicleExit(Transform vehicle)
        {
            if (vehicle != null)
            {
                occupyingVehicles.Remove(vehicle);
            }
        }

        /// <summary>
        /// Retorna o semáforo associado à faixa de entrada informada.
        /// </summary>
        public TrafficSignal GetSignalForLane(TrafficLane lane)
        {
            if (lane == null || trafficSignals == null) return null;

            // Busca primeiro pelo waypoint final (linha de retenção)
            if (lane.LastWaypoint != null && lane.LastWaypoint.AssociatedSignal != null)
            {
                return lane.LastWaypoint.AssociatedSignal;
            }

            // Fallback para semáforo da mesma fase
            int expectedPhase = priorityLanes.Contains(lane) ? 0 : 1;
            return trafficSignals.Find(s => s != null && s.phaseGroup == expectedPhase);
        }

        public void AddEntryLane(TrafficLane lane)
        {
            if (lane != null && !entryLanes.Contains(lane))
            {
                entryLanes.Add(lane);
            }
        }

        public void AddExitLane(TrafficLane lane)
        {
            if (lane != null && !exitLanes.Contains(lane))
            {
                exitLanes.Add(lane);
            }
        }

        public void AddTrafficSignal(TrafficSignal signal)
        {
            if (signal != null && !trafficSignals.Contains(signal))
            {
                trafficSignals.Add(signal);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = priorityType switch
            {
                IntersectionPriorityType.TrafficLightControlled => new Color(0.2f, 0.7f, 1f, 0.4f),
                IntersectionPriorityType.PriorityRoad => new Color(1f, 0.8f, 0.2f, 0.4f),
                IntersectionPriorityType.AllWayStop => new Color(1f, 0.2f, 0.2f, 0.4f),
                IntersectionPriorityType.Roundabout => new Color(0.3f, 1f, 0.5f, 0.4f),
                _ => Color.white
            };

            Gizmos.DrawWireSphere(transform.position, conflictRadius);

            // Linhas de conexão das faixas de entrada para o centro do cruzamento
            Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.6f);
            foreach (var inLane in entryLanes)
            {
                if (inLane != null && inLane.LastWaypoint != null)
                {
                    Gizmos.DrawLine(inLane.LastWaypoint.Position, transform.position);
                }
            }

            // Linhas do centro para as faixas de saída
            Gizmos.color = new Color(0.2f, 1f, 0.9f, 0.6f);
            foreach (var outLane in exitLanes)
            {
                if (outLane != null && outLane.FirstWaypoint != null)
                {
                    Gizmos.DrawLine(transform.position, outLane.FirstWaypoint.Position);
                }
            }
        }
    }
}
