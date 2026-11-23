using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    /// <summary>
    /// Contêiner estrutural da malha viária vetorial de tráfego.
    /// Agrupa vias, faixas, nós de confluência/divergência e cruzamentos urbanos de um setor ou cidade inteira.
    /// Oferece métodos de validação topológica e busca de rotas.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Traffic/Traffic Waypoint Network")]
    public class TrafficWaypointNetwork : MonoBehaviour
    {
        [Header("Identificação da Malha")]
        public string networkName = "Malha Urbana Central";

        [Header("Elementos da Malha")]
        [SerializeField] private List<TrafficLane> registeredLanes = new List<TrafficLane>();
        [SerializeField] private List<TrafficNode> registeredNodes = new List<TrafficNode>();
        [SerializeField] private List<TrafficIntersection> registeredIntersections = new List<TrafficIntersection>();

        public IReadOnlyList<TrafficLane> RegisteredLanes => registeredLanes;
        public IReadOnlyList<TrafficNode> RegisteredNodes => registeredNodes;
        public IReadOnlyList<TrafficIntersection> RegisteredIntersections => registeredIntersections;

        /// <summary>
        /// Faz a varredura automática dos componentes filhos para registrar a rede.
        /// </summary>
        [ContextMenu("Scan and Register Children")]
        public void ScanAndRegisterChildren()
        {
            registeredLanes.Clear();
            registeredNodes.Clear();
            registeredIntersections.Clear();

            registeredLanes.AddRange(GetComponentsInChildren<TrafficLane>(true));
            registeredNodes.AddRange(GetComponentsInChildren<TrafficNode>(true));
            registeredIntersections.AddRange(GetComponentsInChildren<TrafficIntersection>(true));

            Debug.Log($"[TrafficWaypointNetwork] Varredura concluída: {registeredLanes.Count} faixas, {registeredNodes.Count} nós, {registeredIntersections.Count} cruzamentos.");
        }

        /// <summary>
        /// Valida a integridade topológica da malha viária (detecta descontinuidades, waypoints órfãos).
        /// </summary>
        public bool ValidateNetwork(out List<string> issues)
        {
            issues = new List<string>();
            bool isValid = true;

            if (registeredLanes.Count == 0)
            {
                issues.Add("Nenhuma faixa de tráfego registrada na malha.");
                isValid = false;
            }

            foreach (var lane in registeredLanes)
            {
                if (lane == null) continue;

                if (lane.Waypoints == null || lane.Waypoints.Count == 0)
                {
                    issues.Add($"Faixa '{lane.laneId}' não possui nenhum waypoint.");
                    isValid = false;
                }
                else
                {
                    for (int i = 0; i < lane.Waypoints.Count; i++)
                    {
                        var wp = lane.Waypoints[i];
                        if (wp == null)
                        {
                            issues.Add($"Faixa '{lane.laneId}' possui referência nula no índice de waypoint {i}.");
                            isValid = false;
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Localiza o waypoint mais próximo de uma determinada posição espacial no mundo.
        /// </summary>
        public TrafficWaypoint GetNearestWaypoint(Vector3 position, float maxRadius = 50f)
        {
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
        /// Localiza a faixa mais próxima de uma determinada coordenada espacial.
        /// </summary>
        public TrafficLane GetNearestLane(Vector3 position, float maxRadius = 30f)
        {
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
    }
}
