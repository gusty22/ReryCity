using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Instância em tempo de execução de uma ocorrência dinâmica no mundo urbano.
    /// Gerencia o ciclo de vida temporal, estado, área de influência e objetos vinculados (cones, viaturas, veículos avariados).
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Event Instance")]
    public class WorldEventInstance : MonoBehaviour
    {
        [Header("Configuração")]
        public WorldEventConfig eventConfig;
        public WorldEventState currentState = WorldEventState.Pending;

        [Header("Localização e Via")]
        [SerializeField] private Vector3 eventPosition;
        [SerializeField] private TrafficLane associatedLane;
        [SerializeField] private RoadBlockage associatedBlockage;

        [Header("Temporizadores")]
        [SerializeField] private float activeTimer = 0f;
        [SerializeField] private float duration = 60f;

        [Header("Objetos Vinculados (Props/Veículos)")]
        [SerializeField] private List<GameObject> spawnedProps = new List<GameObject>();

        // Eventos
        public event Action<WorldEventInstance, WorldEventState> OnEventStateChanged;

        public Vector3 Position => transform.position;
        public TrafficLane AssociatedLane => associatedLane;
        public RoadBlockage AssociatedBlockage => associatedBlockage;
        public float RemainingTime => Mathf.Max(0f, duration - activeTimer);
        public bool IsActive => currentState == WorldEventState.Active || currentState == WorldEventState.Resolving;

        public void Initialize(WorldEventConfig config, Vector3 pos, TrafficLane lane, RoadBlockage blockage = null)
        {
            eventConfig = config;
            eventPosition = pos;
            transform.position = pos;
            associatedLane = lane;
            associatedBlockage = blockage;

            duration = (config != null) ? config.baseDuration : 60f;
            activeTimer = 0f;
            currentState = WorldEventState.Active;

            // Se o evento bloqueia a faixa, aplica a interdição
            if (config != null && config.blocksEntireLane && associatedLane != null)
            {
                associatedLane.isBlocked = true;
            }

            OnEventStateChanged?.Invoke(this, currentState);
        }

        public void Tick(float dt)
        {
            if (!IsActive) return;

            activeTimer += dt;
            if (activeTimer >= duration)
            {
                CompleteEvent();
            }
        }

        public void StartResolving()
        {
            if (currentState == WorldEventState.Active)
            {
                currentState = WorldEventState.Resolving;
                OnEventStateChanged?.Invoke(this, currentState);
            }
        }

        public void CompleteEvent()
        {
            if (currentState == WorldEventState.Completed) return;

            currentState = WorldEventState.Completed;

            // Desbloqueia a faixa vinculada
            if (associatedLane != null)
            {
                associatedLane.isBlocked = false;
            }

            if (associatedBlockage != null)
            {
                associatedBlockage.ClearBlockage();
            }

            OnEventStateChanged?.Invoke(this, currentState);
        }

        public void AddSpawnedProp(GameObject prop)
        {
            if (prop != null && !spawnedProps.Contains(prop))
            {
                spawnedProps.Add(prop);
                prop.transform.SetParent(transform);
            }
        }

        public void ClearProps()
        {
            foreach (var p in spawnedProps)
            {
                if (p != null) Destroy(p);
            }
            spawnedProps.Clear();
        }

        private void OnDrawGizmos()
        {
            if (eventConfig == null) return;

            Gizmos.color = currentState switch
            {
                WorldEventState.Active => new Color(1f, 0.4f, 0f, 0.4f),     // Laranja
                WorldEventState.Resolving => new Color(0.2f, 0.8f, 1f, 0.4f), // Azul claro
                WorldEventState.Completed => new Color(0.2f, 1f, 0.3f, 0.3f), // Verde
                _ => Color.gray
            };

            Gizmos.DrawWireSphere(transform.position, eventConfig.influenceRadius);
        }
    }
}
