using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    /// <summary>
    /// Gerenciador central de navegação e roteamento de tráfego urbano.
    /// Atua como ponto de acesso unificado para consultas espaciais, verificação de prioridades,
    /// estado de semáforos e renderização de depuração (Gizmos) de toda a malha viária.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Traffic/Traffic Navigation Manager")]
    public class TrafficNavigationManager : MonoBehaviour
    {
        public static TrafficNavigationManager Instance { get; private set; }

        [Header("Rede de Tráfego Principal")]
        [SerializeField] private TrafficWaypointNetwork activeNetwork;

        [Header("Configuração de Depuração (Gizmos)")]
        public bool showGizmos = true;
        public bool showLaneTrajectories = true;
        public bool showWaypoints = true;
        public bool showStopLines = true;
        public bool showIntersections = true;
        public bool showTrafficSignals = true;

        [Header("Cores dos Gizmos")]
        public Color laneColor = new Color(0f, 0.7f, 1f, 0.8f);
        public Color stopLineColor = new Color(1f, 0.1f, 0.1f, 0.9f);
        public Color waypointColor = new Color(0.2f, 1f, 0.3f, 0.8f);
        public Color intersectionColor = new Color(0.8f, 0.2f, 1f, 0.4f);

        private readonly List<TrafficLane> registeredLanes = new List<TrafficLane>();
        private readonly List<TrafficIntersection> registeredIntersections = new List<TrafficIntersection>();
        private readonly List<TrafficSignal> registeredSignals = new List<TrafficSignal>();

        public TrafficWaypointNetwork ActiveNetwork => activeNetwork;
        public IReadOnlyList<TrafficLane> RegisteredLanes => registeredLanes;
        public IReadOnlyList<TrafficIntersection> RegisteredIntersections => registeredIntersections;
        public IReadOnlyList<TrafficSignal> RegisteredSignals => registeredSignals;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (activeNetwork == null)
            {
                activeNetwork = FindFirstObjectByType<TrafficWaypointNetwork>();
            }

            RefreshRegistrations();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Atualiza o registro em memória de todas as entidades de tráfego presentes na cena.
        /// </summary>
        [ContextMenu("Refresh Registrations")]
        public void RefreshRegistrations()
        {
            registeredLanes.Clear();
            registeredIntersections.Clear();
            registeredSignals.Clear();

            registeredLanes.AddRange(FindObjectsByType<TrafficLane>(FindObjectsSortMode.None));
            registeredIntersections.AddRange(FindObjectsByType<TrafficIntersection>(FindObjectsSortMode.None));
            registeredSignals.AddRange(FindObjectsByType<TrafficSignal>(FindObjectsSortMode.None));

            if (activeNetwork != null)
            {
                activeNetwork.ScanAndRegisterChildren();
            }

            Debug.Log($"[TrafficNavigationManager] Registro atualizado: {registeredLanes.Count} faixas, {registeredIntersections.Count} cruzamentos, {registeredSignals.Count} semáforos.");
        }

        /// <summary>
        /// Consulta o waypoint mais próximo de uma coordenada global.
        /// </summary>
        public TrafficWaypoint GetNearestWaypoint(Vector3 position, float maxRadius = 60f)
        {
            if (activeNetwork != null)
            {
                return activeNetwork.GetNearestWaypoint(position, maxRadius);
            }

            TrafficWaypoint nearest = null;
            float minSqrDist = maxRadius * maxRadius;

            foreach (var lane in registeredLanes)
            {
                if (lane == null) continue;
                var wp = lane.GetNearestWaypoint(position);
                if (wp == null) continue;

                float sqrDist = (wp.Position - position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = wp;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Consulta a faixa de tráfego mais próxima de uma coordenada global.
        /// </summary>
        public TrafficLane GetNearestLane(Vector3 position, float maxRadius = 40f)
        {
            if (activeNetwork != null)
            {
                return activeNetwork.GetNearestLane(position, maxRadius);
            }

            TrafficLane nearest = null;
            float minSqrDist = maxRadius * maxRadius;

            foreach (var lane in registeredLanes)
            {
                if (lane == null) continue;
                var wp = lane.GetNearestWaypoint(position);
                if (wp == null) continue;

                float sqrDist = (wp.Position - position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = lane;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Localiza um cruzamento à frente na direção de movimento do veículo.
        /// </summary>
        public TrafficIntersection GetIntersectionAhead(Vector3 position, Vector3 forward, float lookAheadDistance = 30f)
        {
            foreach (var intersection in registeredIntersections)
            {
                if (intersection == null) continue;
                Vector3 toInter = intersection.Position - position;
                float dist = toInter.magnitude;

                if (dist <= lookAheadDistance + intersection.conflictRadius)
                {
                    float dot = Vector3.Dot(forward.normalized, toInter.normalized);
                    if (dot > 0.4f)
                    {
                        return intersection;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Avalia permissão de passagem para um veículo diante de um waypoint (ex: linha de retenção ou semáforo).
        /// </summary>
        public bool CanVehicleProceed(Transform vehicle, TrafficWaypoint waypoint)
        {
            if (waypoint == null) return true;

            if (waypoint.isStopLine && waypoint.AssociatedSignal != null)
            {
                if (waypoint.AssociatedSignal.IsRed)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Gizmos de faixas e waypoints
            if (showLaneTrajectories || showWaypoints)
            {
                foreach (var lane in registeredLanes)
                {
                    if (lane == null || lane.Waypoints == null) continue;

                    for (int i = 0; i < lane.Waypoints.Count; i++)
                    {
                        var wp = lane.Waypoints[i];
                        if (wp == null) continue;

                        if (showWaypoints)
                        {
                            Gizmos.color = waypointColor;
                            Gizmos.DrawSphere(wp.Position, 0.3f);
                        }

                        if (showStopLines && wp.isStopLine)
                        {
                            Gizmos.color = stopLineColor;
                            Vector3 side = wp.transform.right * (wp.laneWidth * 0.5f);
                            Gizmos.DrawLine(wp.Position - side, wp.Position + side);
                        }

                        if (showLaneTrajectories && i < lane.Waypoints.Count - 1)
                        {
                            var nextWp = lane.Waypoints[i + 1];
                            if (nextWp != null)
                            {
                                Gizmos.color = laneColor;
                                Gizmos.DrawLine(wp.Position, nextWp.Position);
                            }
                        }
                    }
                }
            }

            // Gizmos de cruzamentos
            if (showIntersections)
            {
                Gizmos.color = intersectionColor;
                foreach (var inter in registeredIntersections)
                {
                    if (inter != null)
                    {
                        Gizmos.DrawWireSphere(inter.Position, inter.conflictRadius);
                    }
                }
            }
        }
    }
}
