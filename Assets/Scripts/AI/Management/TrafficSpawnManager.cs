using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Gerenciador de geração e dispersão de tráfego urbano perimetral.
    /// Monitora a capacidade máxima de NPCs, seleciona faixas desobstruídas e aciona o AIVehiclePool.
    /// Respeita limites estritos de veículos simultâneos para manter 60 FPS no Android.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/Traffic Spawn Manager")]
    public class TrafficSpawnManager : MonoBehaviour
    {
        public static TrafficSpawnManager Instance { get; private set; }

        [Header("Capacidade e Limites de Tráfego")]
        [Tooltip("Quantidade máxima de veículos NPC ativos simultaneamente.")]
        [Range(4, 40)] public int maxActiveVehicles = 16;

        [Tooltip("Intervalo entre tentativas de geração de novos veículos (segundos).")]
        [Range(1f, 10f)] public float spawnInterval = 3.0f;

        [Header("Área de Geração")]
        [Tooltip("Transform de referência (jogador ou câmera).")]
        [SerializeField] private Transform referenceTransform;

        [Range(20f, 60f)] public float minSpawnDistance = 35f;
        [Range(60f, 120f)] public float maxSpawnDistance = 85f;

        [Header("Mistura de Frota (Pesos Estocásticos)")]
        [Range(0.05f, 0.4f)] public float motorcycleRatio = 0.25f;
        [Range(0.1f, 0.6f)] public float economyCarRatio = 0.40f;
        [Range(0.05f, 0.3f)] public float sedanRatio = 0.20f;
        [Range(0.02f, 0.2f)] public float heavyVehicleRatio = 0.15f;

        private float _spawnTimer = 0f;
        private static readonly Collider[] _clearanceBuffer = new Collider[4];

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (referenceTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null) referenceTransform = mainCam.transform;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (AIVehiclePool.Instance == null) return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= spawnInterval)
            {
                _spawnTimer = 0f;
                TrySpawnVehicle();
            }
        }

        public bool TrySpawnVehicle()
        {
            if (AIVehiclePool.Instance.ActiveCount >= maxActiveVehicles)
            {
                return false;
            }

            TrafficLane lane = SelectCandidateLane();
            if (lane == null || lane.Waypoints == null || lane.Waypoints.Count == 0) return false;

            // Busca waypoint de spawn dentro da faixa de distância permitida
            TrafficWaypoint spawnWaypoint = null;
            Vector3 refPos = (referenceTransform != null) ? referenceTransform.position : Vector3.zero;

            foreach (var wp in lane.Waypoints)
            {
                if (wp == null) continue;
                float d = Vector3.Distance(refPos, wp.Position);
                if (d >= minSpawnDistance && d <= maxSpawnDistance)
                {
                    // Verifica se a área está desobstruída
                    int hits = Physics.OverlapSphereNonAlloc(wp.Position, 3.0f, _clearanceBuffer);
                    if (hits == 0)
                    {
                        spawnWaypoint = wp;
                        break;
                    }
                }
            }

            if (spawnWaypoint == null) return false;

            AIVehicleType typeToSpawn = PickRandomVehicleType();
            AIVehicleController vehicle = AIVehiclePool.Instance.GetVehicle(typeToSpawn);
            if (vehicle != null)
            {
                vehicle.ResetVehicle(spawnWaypoint.Position + Vector3.up * 0.2f, spawnWaypoint.transform.rotation, lane);
                return true;
            }

            return false;
        }

        private TrafficLane SelectCandidateLane()
        {
            if (TrafficNavigationManager.Instance != null && TrafficNavigationManager.Instance.RegisteredLanes.Count > 0)
            {
                var lanes = TrafficNavigationManager.Instance.RegisteredLanes;
                int idx = UnityEngine.Random.Range(0, lanes.Count);
                return lanes[idx];
            }
            return null;
        }

        private AIVehicleType PickRandomVehicleType()
        {
            float roll = UnityEngine.Random.value;
            if (roll < motorcycleRatio) return AIVehicleType.CivilMotorcycle;
            roll -= motorcycleRatio;

            if (roll < economyCarRatio) return AIVehicleType.EconomyCar;
            roll -= economyCarRatio;

            if (roll < sedanRatio) return AIVehicleType.Sedan;
            roll -= sedanRatio;

            if (roll < heavyVehicleRatio * 0.5f) return AIVehicleType.Pickup;
            if (roll < heavyVehicleRatio) return AIVehicleType.Truck;

            return AIVehicleType.EconomyCar;
        }
    }
}
