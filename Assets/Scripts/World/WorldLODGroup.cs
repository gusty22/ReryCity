using System;
using UnityEngine;

namespace ProjetoGrau2.World
{
    public enum WorldLODLevel
    {
        LOD0_HighDetail = 0,   // Máxima qualidade, geometria completa, adereços e física
        LOD1_MediumDetail = 1, // Geometria intermediária, adereços culled, física ativa
        LOD2_LowDetail = 2,    // Modelo simplificado/caixa, colisores desativados
        Culled = 3             // Completamente oculto fora da distância de visão
    }

    /// <summary>
    /// Componente leve de nível de detalhe (World LOD System) para edificações e elementos urbanos.
    /// Ajusta renderizadores e desliga colisores físicos em distâncias médias/longas,
    /// aliviando tanto a GPU quanto o PhysX em dispositivos móveis.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World LOD Group")]
    public class WorldLODGroup : MonoBehaviour
    {
        [Header("Distâncias de Transição (Metros)")]
        [Range(20f, 60f)] public float lod0Distance = 35f;
        [Range(60f, 120f)] public float lod1Distance = 80f;
        [Range(100f, 250f)] public float lod2Distance = 150f;

        [Header("Estado Atual")]
        [SerializeField] private WorldLODLevel currentLOD = WorldLODLevel.LOD0_HighDetail;

        [Header("Renderizadores e Colisores")]
        [SerializeField] private Renderer mainRenderer;
        [SerializeField] private Collider mainCollider;

        public WorldLODLevel CurrentLOD => currentLOD;
        public bool IsCulled => currentLOD == WorldLODLevel.Culled;

        private void Awake()
        {
            if (mainRenderer == null) mainRenderer = GetComponent<Renderer>();
            if (mainCollider == null) mainCollider = GetComponent<Collider>();
        }

        public void InitializeDistances(float l0, float l1, float l2)
        {
            lod0Distance = l0;
            lod1Distance = l1;
            lod2Distance = l2;
        }

        /// <summary>
        /// Avalia a distância até a câmera ou jogador e ajusta os estados gráficos e físicos.
        /// </summary>
        public void EvaluateLOD(Vector3 observerPosition)
        {
            float distSqr = (transform.position - observerPosition).sqrMagnitude;
            float l0Sqr = lod0Distance * lod0Distance;
            float l1Sqr = lod1Distance * lod1Distance;
            float l2Sqr = lod2Distance * lod2Distance;

            WorldLODLevel newLOD;
            if (distSqr <= l0Sqr)
            {
                newLOD = WorldLODLevel.LOD0_HighDetail;
            }
            else if (distSqr <= l1Sqr)
            {
                newLOD = WorldLODLevel.LOD1_MediumDetail;
            }
            else if (distSqr <= l2Sqr)
            {
                newLOD = WorldLODLevel.LOD2_LowDetail;
            }
            else
            {
                newLOD = WorldLODLevel.Culled;
            }

            if (newLOD != currentLOD)
            {
                ApplyLOD(newLOD);
            }
        }

        public void ApplyLOD(WorldLODLevel level)
        {
            currentLOD = level;

            if (mainRenderer != null)
            {
                mainRenderer.enabled = (level != WorldLODLevel.Culled);
            }

            // Desativa colisor de física em LOD 2 e Culled para poupar processamento do PhysX
            if (mainCollider != null)
            {
                mainCollider.enabled = (level == WorldLODLevel.LOD0_HighDetail || level == WorldLODLevel.LOD1_MediumDetail);
            }
        }
    }
}
