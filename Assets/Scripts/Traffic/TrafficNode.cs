using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Traffic
{
    public enum TrafficNodeType
    {
        Standard,   // Conexão direta entre vias
        Diverge,    // Bifurcação: 1 entrada -> múltiplas saídas
        Merge,      // Confluência: múltiplas entradas -> 1 saída
        Junction    // Múltiplas entradas e saídas
    }

    /// <summary>
    /// Nó de convergência ou divergência de fluxo na malha vetorial de tráfego.
    /// Conecta o final de uma ou mais faixas de entrada ao início de faixas de saída.
    /// Permite roteamento probabilístico ou direcionado para NPCs e serviços de tráfego.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Traffic/Traffic Node")]
    public class TrafficNode : MonoBehaviour
    {
        [Header("Configuração do Nó")]
        [Tooltip("Identificador único do nó.")]
        public string nodeId = "node_01";

        [Tooltip("Tipo topológico da conexão viária.")]
        public TrafficNodeType nodeType = TrafficNodeType.Standard;

        [Tooltip("Raio de detecção/área de transição do nó em metros.")]
        [Range(1f, 25f)] public float nodeRadius = 5f;

        [Header("Faixas Vinculadas")]
        [Tooltip("Faixas de tráfego que desembocam neste nó.")]
        [SerializeField] private List<TrafficLane> incomingLanes = new List<TrafficLane>();

        [Tooltip("Faixas de tráfego que partem deste nó.")]
        [SerializeField] private List<TrafficLane> outgoingLanes = new List<TrafficLane>();

        [Header("Probabilidades de Saída (Opcional)")]
        [Tooltip("Pesos relativos para escolha aleatória de faixas de saída. Se vazio, a chance é uniforme.")]
        [SerializeField] private List<float> outgoingWeights = new List<float>();

        public IReadOnlyList<TrafficLane> IncomingLanes => incomingLanes;
        public IReadOnlyList<TrafficLane> OutgoingLanes => outgoingLanes;
        public Vector3 Position => transform.position;

        /// <summary>
        /// Registra uma nova faixa de entrada.
        /// </summary>
        public void AddIncomingLane(TrafficLane lane)
        {
            if (lane != null && !incomingLanes.Contains(lane))
            {
                incomingLanes.Add(lane);
            }
        }

        /// <summary>
        /// Registra uma nova faixa de saída com peso opcional.
        /// </summary>
        public void AddOutgoingLane(TrafficLane lane, float weight = 1f)
        {
            if (lane != null && !outgoingLanes.Contains(lane))
            {
                outgoingLanes.Add(lane);
                outgoingWeights.Add(Mathf.Max(0.1f, weight));
            }
        }

        /// <summary>
        /// Seleciona uma faixa de saída baseada em pesos ponderados para decisão de rota de NPC.
        /// </summary>
        public TrafficLane GetRandomOutgoingLane()
        {
            if (outgoingLanes == null || outgoingLanes.Count == 0) return null;
            if (outgoingLanes.Count == 1) return outgoingLanes[0];

            float totalWeight = 0f;
            for (int i = 0; i < outgoingLanes.Count; i++)
            {
                float w = (i < outgoingWeights.Count) ? outgoingWeights[i] : 1f;
                totalWeight += w;
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float accum = 0f;

            for (int i = 0; i < outgoingLanes.Count; i++)
            {
                float w = (i < outgoingWeights.Count) ? outgoingWeights[i] : 1f;
                accum += w;
                if (roll <= accum)
                {
                    return outgoingLanes[i];
                }
            }

            return outgoingLanes[0];
        }

        /// <summary>
        /// Conecta automaticamente os últimos waypoints das faixas de entrada
        /// aos primeiros waypoints das faixas de saída vinculadas.
        /// </summary>
        public void LinkLanesWaypoints()
        {
            foreach (var inLane in incomingLanes)
            {
                if (inLane == null || inLane.LastWaypoint == null) continue;

                foreach (var outLane in outgoingLanes)
                {
                    if (outLane == null || outLane.FirstWaypoint == null) continue;
                    inLane.LastWaypoint.AddNextWaypoint(outLane.FirstWaypoint);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = nodeType switch
            {
                TrafficNodeType.Diverge => new Color(0.2f, 0.8f, 1f, 0.7f),
                TrafficNodeType.Merge => new Color(1f, 0.6f, 0.1f, 0.7f),
                TrafficNodeType.Junction => new Color(0.9f, 0.3f, 0.9f, 0.7f),
                _ => new Color(0.3f, 1f, 0.4f, 0.7f)
            };

            Gizmos.DrawWireSphere(transform.position, nodeRadius);

            // Desenhar linhas direcionais conectando incoming e outgoing
            foreach (var inLane in incomingLanes)
            {
                if (inLane != null && inLane.LastWaypoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(inLane.LastWaypoint.Position, transform.position);
                }
            }

            foreach (var outLane in outgoingLanes)
            {
                if (outLane != null && outLane.FirstWaypoint != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position, outLane.FirstWaypoint.Position);
                }
            }
        }
    }
}
