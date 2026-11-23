using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerenciador de streaming dinâmico de células urbanas (World Streaming System).
    /// Divide o mapa em chunks e controla carregamento prioritário próximo ao jogador
    /// e descarregamento de áreas distantes sem congelamento de quadros.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Streamer")]
    public class WorldStreamer : MonoBehaviour
    {
        public static WorldStreamer Instance { get; private set; }

        [Header("Configurações do Grid")]
        [Tooltip("Tamanho lateral de cada célula/chunk em metros.")]
        [Range(50f, 200f)] public float chunkSize = 100f;

        [Tooltip("Raio de chunks a manter carregados ao redor do jogador (1 = grid 3x3, 2 = grid 5x5).")]
        [Range(1, 3)] public int loadRadiusChunks = 1;

        [Tooltip("Distância máxima a partir da qual um chunk é descarregado (metros).")]
        [Range(150f, 500f)] public float unloadDistance = 220f;

        [Header("Intervalo de Verificação (Throttling)")]
        [Range(0.1f, 1.0f)] public float checkInterval = 0.25f; // 4 Hz

        [Header("Referências")]
        [SerializeField] private Transform playerTransform;

        // Dicionário de Chunks cadastrados
        private readonly Dictionary<WorldChunkCoord, WorldChunk> _chunks = new Dictionary<WorldChunkCoord, WorldChunk>();
        private readonly List<WorldChunk> _activeLoadedChunks = new List<WorldChunk>();

        private float _checkTimer = 0f;
        private WorldChunkCoord _lastPlayerChunkCoord = new WorldChunkCoord(int.MinValue, int.MinValue);

        // Eventos públicos
        public event Action<WorldChunk> OnChunkLoaded;
        public event Action<WorldChunk> OnChunkUnloaded;

        public int LoadedChunkCount => _activeLoadedChunks.Count;
        public int TotalRegisteredChunks => _chunks.Count;
        public float ChunkSize => chunkSize;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            FindPlayer();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            FindPlayer();
            RegisterSceneChunks();
            UpdateStreamingImmediate();
        }

        private void FindPlayer()
        {
            if (playerTransform == null)
            {
                var player = FindFirstObjectByType<MotorcycleController>();
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
        }

        public void RegisterChunk(WorldChunk chunk)
        {
            if (chunk == null) return;
            _chunks[chunk.Coord] = chunk;
        }

        public void UnregisterChunk(WorldChunk chunk)
        {
            if (chunk == null) return;
            _chunks.Remove(chunk.Coord);
            _activeLoadedChunks.Remove(chunk);
        }

        private void RegisterSceneChunks()
        {
            var sceneChunks = FindObjectsByType<WorldChunk>(FindObjectsSortMode.None);
            for (int i = 0; i < sceneChunks.Length; i++)
            {
                RegisterChunk(sceneChunks[i]);
            }
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            _checkTimer += Time.deltaTime;
            if (_checkTimer >= checkInterval)
            {
                _checkTimer = 0f;
                CheckPlayerMovementAndStream();
            }
        }

        private void CheckPlayerMovementAndStream()
        {
            Vector3 playerPos = playerTransform.position;
            WorldChunkCoord currentCoord = WorldChunkCoord.FromWorldPosition(playerPos, chunkSize);

            if (currentCoord != _lastPlayerChunkCoord)
            {
                _lastPlayerChunkCoord = currentCoord;
                UpdateStreaming(playerPos);
            }
        }

        public void UpdateStreamingImmediate()
        {
            Vector3 playerPos = (playerTransform != null) ? playerTransform.position : Vector3.zero;
            _lastPlayerChunkCoord = WorldChunkCoord.FromWorldPosition(playerPos, chunkSize);
            UpdateStreaming(playerPos);
        }

        public void UpdateStreaming(Vector3 playerPos)
        {
            WorldChunkCoord center = WorldChunkCoord.FromWorldPosition(playerPos, chunkSize);

            // 1. Carrega células dentro do raio
            for (int dx = -loadRadiusChunks; dx <= loadRadiusChunks; dx++)
            {
                for (int dz = -loadRadiusChunks; dz <= loadRadiusChunks; dz++)
                {
                    WorldChunkCoord targetCoord = new WorldChunkCoord(center.x + dx, center.z + dz);
                    if (_chunks.TryGetValue(targetCoord, out var chunk))
                    {
                        if (!chunk.IsLoaded)
                        {
                            chunk.Load();
                            _activeLoadedChunks.Add(chunk);
                            OnChunkLoaded?.Invoke(chunk);
                        }
                    }
                }
            }

            // 2. Descarrega células além da distância de corte
            float unloadDistSqr = unloadDistance * unloadDistance;
            for (int i = _activeLoadedChunks.Count - 1; i >= 0; i--)
            {
                var chunk = _activeLoadedChunks[i];
                if (chunk == null)
                {
                    _activeLoadedChunks.RemoveAt(i);
                    continue;
                }

                float distSqr = (chunk.CenterPosition - playerPos).sqrMagnitude;
                if (distSqr > unloadDistSqr)
                {
                    chunk.Unload();
                    _activeLoadedChunks.RemoveAt(i);
                    OnChunkUnloaded?.Invoke(chunk);
                }
            }
        }

        public bool IsChunkLoaded(WorldChunkCoord coord)
        {
            if (_chunks.TryGetValue(coord, out var chunk))
            {
                return chunk.IsLoaded;
            }
            return false;
        }

        public WorldChunk GetChunk(WorldChunkCoord coord)
        {
            _chunks.TryGetValue(coord, out var chunk);
            return chunk;
        }
    }
}
