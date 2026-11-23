using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    public enum TrafficLODTier
    {
        LOD0_Near,      // < 35m: Sensores a 20 Hz, física completa
        LOD1_Medium,    // 35m a 75m: Sensores a 5 Hz, física padrão
        LOD2_Far        // > 75m: Sensores a 1 Hz, simplificação máxima
    }

    /// <summary>
    /// Gerenciador de níveis de detalhe comportamental e sensorial para tráfego urbano.
    /// Modula as taxas de varredura de sensores e despawna veículos distantes para garantir 60 FPS no mobile.
    /// Utiliza escalonamento temporal distribuído (time-slicing).
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/Traffic LOD Manager")]
    public class TrafficLODManager : MonoBehaviour
    {
        public static TrafficLODManager Instance { get; private set; }

        [Header("Ponto de Referência")]
        [Tooltip("Transform do jogador ou câmera principal para cálculo de distâncias.")]
        [SerializeField] private Transform referenceTransform;

        [Header("Limiares de Distância (Metros)")]
        [Range(20f, 60f)] public float lod0Distance = 35f;
        [Range(50f, 120f)] public float lod1Distance = 75f;
        [Range(80f, 200f)] public float despawnDistance = 120f;

        [Header("Frequências dos Sensores por Tier")]
        public float lod0SensorInterval = 0.05f; // 20 Hz
        public float lod1SensorInterval = 0.20f; // 5 Hz
        public float lod2SensorInterval = 1.00f; // 1 Hz

        private int _currentBatchIndex = 0;
        private const int BATCH_SIZE = 4; // Processa até 4 veículos por frame

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
            if (referenceTransform == null || AIVehiclePool.Instance == null) return;

            var vehicles = AIVehiclePool.Instance.ActiveVehicles;
            if (vehicles == null || vehicles.Count == 0) return;

            // Atualização fatiada (Time-Sliced) para evitar picos de CPU
            int count = vehicles.Count;
            for (int i = 0; i < BATCH_SIZE && i < count; i++)
            {
                _currentBatchIndex = (_currentBatchIndex + 1) % count;
                var v = vehicles[_currentBatchIndex];
                if (v == null || !v.gameObject.activeInHierarchy) continue;

                EvaluateVehicleLOD(v);
            }
        }

        private void EvaluateVehicleLOD(AIVehicleController vehicle)
        {
            float dist = Vector3.Distance(referenceTransform.position, vehicle.transform.position);

            // Despawn automático por distância excessiva
            if (dist > despawnDistance)
            {
                AIVehiclePool.Instance.ReturnVehicle(vehicle);
                return;
            }

            var sensor = vehicle.GetComponent<AIVehicleSensor>();
            if (sensor == null) return;

            if (dist < lod0Distance)
            {
                sensor.SetUpdateInterval(lod0SensorInterval);
            }
            else if (dist < lod1Distance)
            {
                sensor.SetUpdateInterval(lod1SensorInterval);
            }
            else
            {
                sensor.SetUpdateInterval(lod2SensorInterval);
            }
        }

        public void SetReferenceTransform(Transform refTrans)
        {
            referenceTransform = refTrans;
        }
    }
}
