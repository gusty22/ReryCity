using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    /// <summary>
    /// Ponto de passagem vetorial individual em uma faixa de tráfego.
    /// Contém posição espacial, velocidade recomendada, conexões de continuidade e bifurcação.
    /// </summary>
    [Serializable]
    public class TrafficWaypoint : MonoBehaviour
    {
        [Header("Configuração do Ponto")]
        [Tooltip("Velocidade limite permitida neste ponto (km/h).")]
        [Range(10f, 120f)] public float speedLimitKmh = 60f;

        [Tooltip("Largura útil da faixa neste ponto (metros).")]
        [Range(2.5f, 6.0f)] public float laneWidth = 3.5f;

        [Tooltip("Indica se este ponto é uma linha de retenção/parada (Stop Line) de semáforo ou cruzamento.")]
        public bool isStopLine = false;

        [Header("Conexões de Rota")]
        [SerializeField] private List<TrafficWaypoint> nextWaypoints = new List<TrafficWaypoint>();
        [SerializeField] private List<TrafficWaypoint> previousWaypoints = new List<TrafficWaypoint>();

        [Header("Vínculos Opcionais")]
        [Tooltip("Semáforo associado se este ponto for uma linha de retenção.")]
        [SerializeField] private TrafficSignal associatedSignal;
        [Tooltip("Faixa de tráfego pai a que pertence este waypoint.")]
        [SerializeField] private TrafficLane parentLane;

        public IReadOnlyList<TrafficWaypoint> NextWaypoints => nextWaypoints;
        public IReadOnlyList<TrafficWaypoint> PreviousWaypoints => previousWaypoints;
        public TrafficSignal AssociatedSignal => associatedSignal;
        public TrafficLane ParentLane => parentLane;

        public Vector3 Position => transform.position;
        public Vector3 Forward => transform.forward;

        public void SetParentLane(TrafficLane lane)
        {
            parentLane = lane;
        }

        public void SetAssociatedSignal(TrafficSignal signal)
        {
            associatedSignal = signal;
        }

        public void AddNextWaypoint(TrafficWaypoint waypoint)
        {
            if (waypoint != null && !nextWaypoints.Contains(waypoint))
            {
                nextWaypoints.Add(waypoint);
                if (!waypoint.previousWaypoints.Contains(this))
                {
                    waypoint.previousWaypoints.Add(this);
                }
            }
        }

        public void RemoveNextWaypoint(TrafficWaypoint waypoint)
        {
            if (waypoint != null && nextWaypoints.Contains(waypoint))
            {
                nextWaypoints.Remove(waypoint);
                waypoint.previousWaypoints.Remove(this);
            }
        }

        /// <summary>
        /// Retorna um próximo ponto aleatório ou o primeiro disponível para bifurcações.
        /// </summary>
        public TrafficWaypoint GetNextWaypoint(int branchIndex = 0)
        {
            if (nextWaypoints == null || nextWaypoints.Count == 0) return null;
            if (branchIndex >= 0 && branchIndex < nextWaypoints.Count)
            {
                return nextWaypoints[branchIndex];
            }
            return nextWaypoints[0];
        }

        /// <summary>
        /// Verifica se o veículo deve parar neste ponto por sinal vermelho ou retenção.
        /// </summary>
        public bool MustStop()
        {
            if (!isStopLine) return false;
            if (associatedSignal != null)
            {
                return associatedSignal.CurrentState != TrafficSignalState.Green;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isStopLine ? Color.red : Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.35f);

            // Linha de direção para os próximos waypoints
            if (nextWaypoints != null)
            {
                Gizmos.color = isStopLine ? Color.red : new Color(0.2f, 0.8f, 1f, 0.8f);
                foreach (var next in nextWaypoints)
                {
                    if (next != null)
                    {
                        Gizmos.DrawLine(transform.position, next.transform.position);
                    }
                }
            }
        }
    }
}
