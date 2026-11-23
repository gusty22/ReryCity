using System;
using UnityEngine;

namespace ProjetoGrau2.Police
{
    public enum PoliceVehicleType
    {
        CruiserSUV,         // Viatura 4x4 (Hilux / Trailblazer / Força Tática)
        RocamMotorcycle,    // Motocicleta de Patrulhamento Tático (Tiger 800 / XT 660)
        HighwayInterceptor  // Sedã de Patrulhamento Rodoviário
    }

    /// <summary>
    /// Especificação física e dimensional de viaturas policiais e motocicletas ROCAM.
    /// Contém propriedades de aceleração de perseguição, capacidade de frenagem e sinais acústicos.
    /// </summary>
    [CreateAssetMenu(fileName = "New_PoliceVehicleData", menuName = "Projeto Grau 2/Police/Police Vehicle Data")]
    public class PoliceVehicleData : ScriptableObject
    {
        [Header("Classificação e Identificação")]
        public PoliceVehicleType vehicleType = PoliceVehicleType.CruiserSUV;
        public string modelName = "Viatura Policial Força Tática";

        [Header("Propriedades Físicas (Rigidbody)")]
        [Tooltip("Massa da viatura em quilogramas (kg).")]
        [Range(150f, 4000f)] public float mass = 2150f;

        [Tooltip("Arrasto linear (Air Drag).")]
        [Range(0.02f, 0.5f)] public float airDrag = 0.08f;

        [Tooltip("Arrasto angular (Angular Drag).")]
        [Range(0.1f, 5.0f)] public float angularDrag = 2.5f;

        [Header("Dimensões do Colisor Proxy")]
        public Vector3 colliderSize = new Vector3(1.95f, 1.85f, 5.1f);
        public Vector3 colliderCenter = new Vector3(0f, 0.95f, 0f);
        [Range(1.3f, 3.8f)] public float wheelbase = 3.05f;

        [Header("Desempenho de Perseguição")]
        [Tooltip("Velocidade máxima em perseguição aberta (km/h).")]
        [Range(100f, 260f)] public float topSpeedKmh = 185f;

        [Tooltip("Força base de aceleração em Newtons (N).")]
        [Range(2000f, 15000f)] public float accelerationForce = 5800f;

        [Tooltip("Multiplicador de aceleração de emergência em perseguição ativa.")]
        [Range(1.0f, 1.6f)] public float pursuitBoostMultiplier = 1.25f;

        [Tooltip("Força máxima de frenagem em Newtons (N).")]
        [Range(3000f, 25000f)] public float brakeForce = 11500f;

        [Header("Esterçamento e Dinâmica")]
        [Range(20f, 45f)] public float maxSteerAngle = 36f;
        [Range(40f, 160f)] public float steerSpeed = 95f;
        [Range(0.3f, 1.0f)] public float highSpeedSteerScale = 0.55f;

        [Header("Específico ROCAM (Duas Rodas)")]
        public bool isTwoWheeled = false;
        [Range(0f, 45f)] public float maxLeanAngle = 32f;
        [Range(20f, 120f)] public float leanSpeed = 60f;

        [Header("Sinalização e Sirene")]
        public bool hasSiren = true;
        public float sirenAudibilityRadius = 60f;
    }
}
