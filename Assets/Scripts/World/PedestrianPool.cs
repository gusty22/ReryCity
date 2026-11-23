using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Object Pool dedicado para pedestres NPC (Zero GC).
    /// Mantém uma fila de instâncias pré-alocadas de pedestres, eliminando
    /// chamadas a Instantiate/Destroy durante o gameplay em dispositivos móveis.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/Pedestrian Pool")]
    public class PedestrianPool : MonoBehaviour
    {
        public static PedestrianPool Instance { get; private set; }

        [Header("Configurações do Pool")]
        [Tooltip("Quantidade máxima de instâncias pré-alocadas em memória.")]
        [Range(10, 50)] public int poolCapacity = 25;

        [SerializeField] private GameObject pedestrianPrefab;

        private readonly Queue<PedestrianController> _availableQueue = new Queue<PedestrianController>();
        private readonly List<PedestrianController> _allInstances = new List<PedestrianController>();

        public int AvailableCount => _availableQueue.Count;
        public int ActiveCount => _allInstances.Count - _availableQueue.Count;
        public int TotalInstances => _allInstances.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            PrewarmPool();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void PrewarmPool()
        {
            for (int i = 0; i < poolCapacity; i++)
            {
                CreateNewInstance();
            }
        }

        private PedestrianController CreateNewInstance()
        {
            GameObject go;
            if (pedestrianPrefab != null)
            {
                go = Instantiate(pedestrianPrefab, transform);
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.transform.localScale = new Vector3(0.6f, 1.75f, 0.6f);
                go.transform.SetParent(transform, true);
            }

            go.name = $"Pedestrian_NPC_{_allInstances.Count}";
            var controller = go.GetComponent<PedestrianController>();
            if (controller == null)
            {
                controller = go.AddComponent<PedestrianController>();
            }

            controller.ResetPedestrian();
            _allInstances.Add(controller);
            _availableQueue.Enqueue(controller);
            return controller;
        }

        public PedestrianController GetPedestrian(Vector3 spawnPos, Vector3 targetPos)
        {
            PedestrianController controller;
            if (_availableQueue.Count > 0)
            {
                controller = _availableQueue.Dequeue();
            }
            else
            {
                // Se o pool esgotou e atingiu o teto, reutiliza a instância mais antiga sem estourar o limite
                controller = _allInstances[0];
            }

            controller.Initialize(spawnPos, targetPos);
            return controller;
        }

        public void ReleasePedestrian(PedestrianController controller)
        {
            if (controller == null) return;
            controller.ResetPedestrian();

            if (!_availableQueue.Contains(controller))
            {
                _availableQueue.Enqueue(controller);
            }
        }
    }
}
