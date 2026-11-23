using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerenciador de densidade e circulação de pedestres NPC (World Population System).
    /// Controla a quantidade de pedestres ativos no entorno do jogador, despawna automaticamente
    /// instâncias além do raio de visibilidade e as devolve ao PedestrianPool.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Population Manager")]
    public class WorldPopulationManager : MonoBehaviour
    {
        public static WorldPopulationManager Instance { get; private set; }

        [Header("Limites de Densidade")]
        [Tooltip("Quantidade máxima de pedestres ativos simultâneos (Otimização Android).")]
        [Range(5, 30)] public int maxActivePedestrians = 15;

        [Header("Distâncias de Operação")]
        [Range(10f, 35f)] public float minSpawnDistance = 15f;
        [Range(30f, 60f)] public float maxSpawnDistance = 45f;
        [Range(40f, 80f)] public float despawnDistance = 55f;

        [Header("Intervalo de Spawn")]
        [Range(1f, 10f)] public float spawnInterval = 3.5f;

        [Header("Referências")]
        [SerializeField] private Transform playerTransform;

        private readonly List<PedestrianController> _activePedestrians = new List<PedestrianController>();
        private float _spawnTimer = 0f;

        public int ActivePedestrianCount => _activePedestrians.Count;

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

        private void Update()
        {
            float dt = Time.deltaTime;

            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            CullDistantPedestrians();

            _spawnTimer += dt;
            if (_spawnTimer >= spawnInterval)
            {
                _spawnTimer = 0f;
                TrySpawnPedestrianNearPlayer();
            }
        }

        private void CullDistantPedestrians()
        {
            Vector3 playerPos = playerTransform.position;
            float despawnSqr = despawnDistance * despawnDistance;

            for (int i = _activePedestrians.Count - 1; i >= 0; i--)
            {
                var ped = _activePedestrians[i];
                if (ped == null || !ped.gameObject.activeInHierarchy)
                {
                    _activePedestrians.RemoveAt(i);
                    continue;
                }

                float distSqr = (ped.transform.position - playerPos).sqrMagnitude;
                if (distSqr > despawnSqr)
                {
                    if (PedestrianPool.Instance != null)
                    {
                        PedestrianPool.Instance.ReleasePedestrian(ped);
                    }
                    else
                    {
                        ped.ResetPedestrian();
                    }
                    _activePedestrians.RemoveAt(i);
                }
            }
        }

        private void TrySpawnPedestrianNearPlayer()
        {
            if (_activePedestrians.Count >= maxActivePedestrians) return;
            if (PedestrianPool.Instance == null) return;

            Vector3 playerPos = playerTransform.position;
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPos = playerPos + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Alvo de caminhada ligeiramente deslocado
            Vector3 targetPos = spawnPos + new Vector3(UnityEngine.Random.Range(-15f, 15f), 0f, UnityEngine.Random.Range(-15f, 15f));

            var ped = PedestrianPool.Instance.GetPedestrian(spawnPos, targetPos);
            if (ped != null && !_activePedestrians.Contains(ped))
            {
                _activePedestrians.Add(ped);
            }
        }
    }
}
