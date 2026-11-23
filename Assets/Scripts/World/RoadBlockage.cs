using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerenciador de bloqueios viários, obras e interdições físicas de faixas.
    /// Interdita logicamente a TrafficLane e posiciona barreiras físicas e cones de sinalização
    /// para desviar o tráfego civil e criar desafios dinâmicos para a pilotagem do jogador.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/Road Blockage")]
    public class RoadBlockage : MonoBehaviour
    {
        [Header("Faixas Vinculadas")]
        [SerializeField] private TrafficLane blockedLane;
        [SerializeField] private TrafficLane detourLane;

        [Header("Configuração dos Obstáculos Físicos")]
        [Tooltip("Quantidade de cones de trânsito distribuídos ao longo da interdição.")]
        [Range(2, 12)] public int coneCount = 5;

        [Tooltip("Comprimento da área interditada em metros.")]
        [Range(10f, 60f)] public float blockageLength = 25f;

        [Header("Elementos Gerados")]
        [SerializeField] private List<GameObject> spawnedObstacles = new List<GameObject>();

        public TrafficLane BlockedLane => blockedLane;
        public TrafficLane DetourLane => detourLane;
        public bool IsActive => blockedLane != null && blockedLane.isBlocked;

        public void ApplyBlockage(TrafficLane lane, TrafficLane detour = null)
        {
            blockedLane = lane;
            detourLane = detour;

            if (blockedLane != null)
            {
                blockedLane.isBlocked = true;
            }

            SpawnPhysicalObstacles();
        }

        public void ClearBlockage()
        {
            if (blockedLane != null)
            {
                blockedLane.isBlocked = false;
            }

            foreach (var obs in spawnedObstacles)
            {
                if (obs != null) Destroy(obs);
            }
            spawnedObstacles.Clear();
        }

        private void SpawnPhysicalObstacles()
        {
            // Limpa instâncias anteriores se houver
            foreach (var obs in spawnedObstacles)
            {
                if (obs != null) Destroy(obs);
            }
            spawnedObstacles.Clear();

            Vector3 basePos = transform.position;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // 1. Barreira Central / Cavalete de Obra (BoxCollider com física estática)
            GameObject barrier = new GameObject("RoadBlockage_Barrier_Board");
            barrier.transform.position = basePos + Vector3.up * 0.5f;
            barrier.transform.rotation = transform.rotation;
            barrier.transform.SetParent(transform);

            BoxCollider bCol = barrier.AddComponent<BoxCollider>();
            bCol.size = new Vector3(3.2f, 1.0f, 0.4f);

            // Visual representativo
            GameObject bVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bVisual.name = "Barrier_Visual";
            bVisual.transform.SetParent(barrier.transform);
            bVisual.transform.localPosition = Vector3.zero;
            bVisual.transform.localRotation = Quaternion.identity;
            bVisual.transform.localScale = bCol.size;
            Collider childCol = bVisual.GetComponent<Collider>();
            if (childCol != null) Destroy(childCol);

            spawnedObstacles.Add(barrier);

            // 2. Fila Diagonal de Cones de Sinalização
            float spacing = blockageLength / Mathf.Max(1, coneCount);
            for (int i = 0; i < coneCount; i++)
            {
                float t = (float)i / (coneCount - 1);
                // Cones em disposição diagonal orientando o desvio lateral
                Vector3 conePos = basePos - forward * (i * spacing * 0.5f) + right * ((t - 0.5f) * 2.5f);

                GameObject cone = new GameObject($"TrafficCone_{i + 1}");
                cone.transform.position = conePos;
                cone.transform.SetParent(transform);

                BoxCollider cCol = cone.AddComponent<BoxCollider>();
                cCol.size = new Vector3(0.5f, 0.8f, 0.5f);
                cCol.center = new Vector3(0f, 0.4f, 0f);

                GameObject cVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cVisual.name = "Cone_Visual";
                cVisual.transform.SetParent(cone.transform);
                cVisual.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                cVisual.transform.localRotation = Quaternion.identity;
                cVisual.transform.localScale = new Vector3(0.35f, 0.4f, 0.35f);
                Collider coneChildCol = cVisual.GetComponent<Collider>();
                if (coneChildCol != null) Destroy(coneChildCol);

                spawnedObstacles.Add(cone);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0f, 0.6f); // Laranja avermelhado
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(3.5f, 1.2f, blockageLength));

            if (detourLane != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, detourLane.transform.position);
            }
        }
    }
}
