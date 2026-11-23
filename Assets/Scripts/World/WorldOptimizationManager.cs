using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerenciador mestre de otimização de mundo aberto para Android (World Optimization Manager).
    /// Controla orçamento de memória, tetos de entidades ativas simultâneas (carros civis, viaturas, pedestres, chunks),
    /// e distribui a atualização de distâncias de LOD via Time-Slicing (fatiamento de tempo entre frames).
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Optimization Manager")]
    public class WorldOptimizationManager : MonoBehaviour
    {
        public static WorldOptimizationManager Instance { get; private set; }

        [Header("Metas de Desempenho")]
        [Tooltip("Taxa de quadros alvo para o runtime mobile.")]
        public int targetFrameRate = 60;

        [Header("Orçamento de Entidades Ativas (Android Budget)")]
        [Range(4, 20)] public int maxActiveCars = 12;
        [Range(5, 30)] public int maxActivePedestrians = 15;
        [Range(1, 6)] public int maxActivePolice = 4;
        [Range(1, 16)] public int maxActiveLoadedChunks = 9;

        [Header("Time-Slicing de LOD")]
        [Tooltip("Quantidade de lotes para divisão da avaliação de LOD por frame.")]
        [Range(2, 8)] public int timeSliceBuckets = 4;

        [Header("Referências")]
        [SerializeField] private Transform playerTransform;

        private readonly List<WorldLODGroup> _registeredLODs = new List<WorldLODGroup>();
        private int _currentBucketIndex = 0;

        public int RegisteredLODCount => _registeredLODs.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Application.targetFrameRate = targetFrameRate;
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
            RegisterSceneLODGroups();
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

        public void RegisterLODGroup(WorldLODGroup lod)
        {
            if (lod != null && !_registeredLODs.Contains(lod))
            {
                _registeredLODs.Add(lod);
            }
        }

        public void UnregisterLODGroup(WorldLODGroup lod)
        {
            if (lod != null)
            {
                _registeredLODs.Remove(lod);
            }
        }

        private void RegisterSceneLODGroups()
        {
            var lods = FindObjectsByType<WorldLODGroup>(FindObjectsSortMode.None);
            for (int i = 0; i < lods.Length; i++)
            {
                RegisterLODGroup(lods[i]);
            }
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            UpdateTimeSlicedLOD();
        }

        /// <summary>
        /// Avalia apenas uma fração dos objetos por frame para evitar picos de CPU no Android.
        /// </summary>
        private void UpdateTimeSlicedLOD()
        {
            int total = _registeredLODs.Count;
            if (total == 0) return;

            Vector3 playerPos = playerTransform.position;
            int batchSize = Mathf.CeilToInt((float)total / timeSliceBuckets);
            int startIndex = _currentBucketIndex * batchSize;
            int endIndex = Mathf.Min(startIndex + batchSize, total);

            for (int i = startIndex; i < endIndex; i++)
            {
                var lod = _registeredLODs[i];
                if (lod != null && lod.gameObject.activeInHierarchy)
                {
                    lod.EvaluateLOD(playerPos);
                }
            }

            _currentBucketIndex = (_currentBucketIndex + 1) % timeSliceBuckets;
        }

        /// <summary>
        /// Força a avaliação imediata de todos os LODs (útil para testes unitários ou teletransporte).
        /// </summary>
        public void EvaluateAllLODsImmediate(Vector3 observerPos)
        {
            for (int i = 0; i < _registeredLODs.Count; i++)
            {
                var lod = _registeredLODs[i];
                if (lod != null && lod.gameObject.activeInHierarchy)
                {
                    lod.EvaluateLOD(observerPos);
                }
            }
        }
    }
}
