using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerenciador central de ocorrências dinâmicas no mundo urbano (World Event Manager).
    /// Controla a criação procedural de acidentes, obras, panes e blitzes policiais.
    /// Impede sobreposição espacial de eventos, gerencia densidade máxima e aciona o despacho policial.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Event Manager")]
    public class WorldEventManager : MonoBehaviour
    {
        public static WorldEventManager Instance { get; private set; }

        [Header("Catálogo de Configurações")]
        [SerializeField] private List<WorldEventConfig> eventCatalog = new List<WorldEventConfig>();

        [Header("Capacidade e Limites de Densidade")]
        [Tooltip("Quantidade máxima de ocorrências ativas simultaneamente.")]
        [Range(1, 6)] public int maxActiveEvents = 3;

        [Tooltip("Intervalo entre tentativas de geração de novos eventos (segundos).")]
        [Range(5f, 60f)] public float eventSpawnInterval = 25f;

        [Header("Área de Geração (Raio em relação ao jogador)")]
        [Tooltip("Distância mínima do jogador para gerar um evento (fora da visão imediata).")]
        [Range(20f, 60f)] public float minSpawnDistance = 35f;

        [Tooltip("Distância máxima do jogador para gerar um evento.")]
        [Range(60f, 150f)] public float maxSpawnDistance = 90f;

        [Header("Ponto de Referência")]
        [SerializeField] private Transform referenceTransform;

        // Ocorrências ativas
        private readonly List<WorldEventInstance> _activeEvents = new List<WorldEventInstance>();
        private float _spawnTimer = 0f;

        // Eventos públicos
        public event Action<WorldEventInstance> OnEventStarted;
        public event Action<WorldEventInstance> OnEventFinished;

        public IReadOnlyList<WorldEventInstance> ActiveEvents => _activeEvents;
        public int ActiveCount => _activeEvents.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            FindReferenceTransform();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void FindReferenceTransform()
        {
            if (referenceTransform == null)
            {
                var player = FindFirstObjectByType<MotorcycleController>();
                if (player != null) referenceTransform = player.transform;
                else if (Camera.main != null) referenceTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            UpdateActiveEvents(dt);
            UpdateProceduralSpawner(dt);
        }

        private void UpdateActiveEvents(float dt)
        {
            for (int i = _activeEvents.Count - 1; i >= 0; i--)
            {
                var evt = _activeEvents[i];
                if (evt == null)
                {
                    _activeEvents.RemoveAt(i);
                    continue;
                }

                evt.Tick(dt);

                if (evt.currentState == WorldEventState.Completed || evt.currentState == WorldEventState.Cancelled)
                {
                    TerminateEvent(evt);
                }
            }
        }

        private void UpdateProceduralSpawner(float dt)
        {
            _spawnTimer += dt;
            if (_spawnTimer >= eventSpawnInterval)
            {
                _spawnTimer = 0f;
                if (_activeEvents.Count < maxActiveEvents)
                {
                    TrySpawnRandomEvent();
                }
            }
        }

        /// <summary>
        /// Tenta instanciar um evento procedural em uma faixa da malha viária desobstruída.
        /// </summary>
        public WorldEventInstance TrySpawnRandomEvent()
        {
            if (eventCatalog == null || eventCatalog.Count == 0) return null;
            if (_activeEvents.Count >= maxActiveEvents) return null;

            int idx = UnityEngine.Random.Range(0, eventCatalog.Count);
            WorldEventConfig config = eventCatalog[idx];
            if (config == null) return null;

            TrafficLane candidateLane = SelectCandidateLane();
            if (candidateLane == null || candidateLane.Waypoints == null || candidateLane.Waypoints.Count == 0) return null;

            Vector3 refPos = (referenceTransform != null) ? referenceTransform.position : Vector3.zero;

            // Busca waypoint na faixa dentro da distância estipulada
            TrafficWaypoint targetWp = null;
            foreach (var wp in candidateLane.Waypoints)
            {
                if (wp == null) continue;
                float d = Vector3.Distance(refPos, wp.Position);
                if (d >= minSpawnDistance && d <= maxSpawnDistance)
                {
                    if (IsAreaFreeOfEvents(wp.Position, config.minSpacingFromOtherEvents))
                    {
                        targetWp = wp;
                        break;
                    }
                }
            }

            if (targetWp == null) return null;

            return SpawnEvent(config, targetWp.Position, candidateLane);
        }

        /// <summary>
        /// Instancia um evento configurado em uma coordenada específica da via.
        /// </summary>
        public WorldEventInstance SpawnEvent(WorldEventConfig config, Vector3 position, TrafficLane lane)
        {
            if (config == null) return null;

            GameObject eventGo = new GameObject($"WorldEvent_{config.eventType}_{_activeEvents.Count + 1}");
            eventGo.transform.position = position;
            eventGo.transform.SetParent(transform);

            WorldEventInstance instance = eventGo.AddComponent<WorldEventInstance>();
            instance.Initialize(config, position, lane);

            _activeEvents.Add(instance);
            OnEventStarted?.Invoke(instance);

            // Se o evento requerer polícia, notifica o despachante
            if (config.requiresPoliceDispatch && PoliceDispatchManager.Instance != null)
            {
                PoliceDispatchManager.Instance.DispatchIncident(instance);
            }

            return instance;
        }

        /// <summary>
        /// Finaliza e destrói uma ocorrência, desobstruindo vias e limpando props.
        /// </summary>
        public void TerminateEvent(WorldEventInstance instance)
        {
            if (instance == null) return;

            instance.CompleteEvent();
            instance.ClearProps();
            _activeEvents.Remove(instance);
            OnEventFinished?.Invoke(instance);

            Destroy(instance.gameObject);
        }

        /// <summary>
        /// Verifica se uma coordenada está suficientemente distante de outras ocorrências ativas.
        /// </summary>
        public bool IsAreaFreeOfEvents(Vector3 pos, float minDistance)
        {
            float minSqr = minDistance * minDistance;
            foreach (var evt in _activeEvents)
            {
                if (evt == null) continue;
                if ((evt.Position - pos).sqrMagnitude < minSqr)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Retorna a ocorrência mais próxima de uma posição.
        /// </summary>
        public WorldEventInstance GetNearestEvent(Vector3 pos, float maxRadius = 50f)
        {
            WorldEventInstance nearest = null;
            float minSqr = maxRadius * maxRadius;

            foreach (var evt in _activeEvents)
            {
                if (evt == null || !evt.IsActive) continue;
                float sqrDist = (evt.Position - pos).sqrMagnitude;
                if (sqrDist < minSqr)
                {
                    minSqr = sqrDist;
                    nearest = evt;
                }
            }

            return nearest;
        }

        private TrafficLane SelectCandidateLane()
        {
            if (TrafficNavigationManager.Instance != null && TrafficNavigationManager.Instance.RegisteredLanes.Count > 0)
            {
                var lanes = TrafficNavigationManager.Instance.RegisteredLanes;
                int r = UnityEngine.Random.Range(0, lanes.Count);
                return lanes[r];
            }
            return null;
        }
    }
}
