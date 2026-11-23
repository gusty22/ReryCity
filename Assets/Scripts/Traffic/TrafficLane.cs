using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    public enum TrafficLaneDirection
    {
        Forward,  // Sentido padrão da via
        Backward  // Sentido oposto (pista de mão dupla)
    }

    /// <summary>
    /// Representa uma faixa individual de rolamento em uma via urbana.
    /// Contém a sequência ordenada de waypoints, limites de velocidade e conexões para mudança de faixa.
    /// </summary>
    public class TrafficLane : MonoBehaviour
    {
        [Header("Identificação da Faixa")]
        public string laneId = "lane_avenida_norte_0";
        public int laneIndex = 0; // 0 = faixa da direita, 1 = faixa da esquerda/ultrapassagem
        public TrafficLaneDirection direction = TrafficLaneDirection.Forward;
        public float speedLimitKmh = 60f;
        public float laneWidth = 3.5f;

        [Header("Interdição e Bloqueios")]
        [Tooltip("Indica se esta faixa está interditada por obras, acidentes ou bloqueios.")]
        public bool isBlocked = false;

        [Header("Sequência de Waypoints")]
        [SerializeField] private List<TrafficWaypoint> waypoints = new List<TrafficWaypoint>();

        [Header("Faixas Adjacentes (Mudança de Faixa)")]
        [Tooltip("Faixa à esquerda para ultrapassagem (se existir).")]
        [SerializeField] private TrafficLane leftAdjacentLane;
        [Tooltip("Faixa à direita para retorno (se existir).")]
        [SerializeField] private TrafficLane rightAdjacentLane;

        public IReadOnlyList<TrafficWaypoint> Waypoints => waypoints;
        public TrafficLane LeftAdjacentLane => leftAdjacentLane;
        public TrafficLane RightAdjacentLane => rightAdjacentLane;

        public TrafficWaypoint FirstWaypoint => (waypoints != null && waypoints.Count > 0) ? waypoints[0] : null;
        public TrafficWaypoint LastWaypoint => (waypoints != null && waypoints.Count > 0) ? waypoints[waypoints.Count - 1] : null;

        private void Awake()
        {
            RebuildWaypointConnections();
        }

        /// <summary>
        /// Reconstrói as conexões lineares automáticas entre waypoints filhos sequenciais.
        /// </summary>
        public void RebuildWaypointConnections()
        {
            if (waypoints == null || waypoints.Count == 0)
            {
                waypoints = new List<TrafficWaypoint>(GetComponentsInChildren<TrafficWaypoint>());
            }

            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] != null)
                {
                    waypoints[i].SetParentLane(this);
                    waypoints[i].speedLimitKmh = speedLimitKmh;
                    waypoints[i].laneWidth = laneWidth;

                    if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                    {
                        waypoints[i].AddNextWaypoint(waypoints[i + 1]);
                    }
                }
            }
        }

        public void AddWaypoint(TrafficWaypoint waypoint)
        {
            if (waypoint != null && !waypoints.Contains(waypoint))
            {
                if (waypoints.Count > 0)
                {
                    waypoints[waypoints.Count - 1].AddNextWaypoint(waypoint);
                }
                waypoints.Add(waypoint);
                waypoint.SetParentLane(this);
            }
        }

        /// <summary>
        /// Encontra o waypoint mais próximo de uma dada coordenada mundial dentro desta faixa.
        /// </summary>
        public TrafficWaypoint GetClosestWaypoint(Vector3 worldPosition)
        {
            if (waypoints == null || waypoints.Count == 0) return null;

            TrafficWaypoint closest = null;
            float minSqrDist = float.MaxValue;

            foreach (var wp in waypoints)
            {
                if (wp == null) continue;
                float sqrDist = (wp.Position - worldPosition).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closest = wp;
                }
            }

            return closest;
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Count < 2) return;

            Gizmos.color = (direction == TrafficLaneDirection.Forward) ? new Color(0f, 0.9f, 1f, 0.75f) : new Color(1f, 0.5f, 0f, 0.75f);

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].Position, waypoints[i + 1].Position);

                    // Desenha marcação lateral da faixa
                    Vector3 mid = (waypoints[i].Position + waypoints[i + 1].Position) * 0.5f;
                    Vector3 dir = (waypoints[i + 1].Position - waypoints[i].Position).normalized;
                    Vector3 normal = Vector3.Cross(Vector3.up, dir) * (laneWidth * 0.5f);

                    Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
                    Gizmos.DrawLine(mid - normal, mid + normal);
                }
            }
        }
    }
}
