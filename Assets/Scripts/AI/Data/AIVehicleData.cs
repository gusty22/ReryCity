using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Especificação física e dimensional parametrizada de uma categoria de veículo NPC.
    /// Define massa, dimensões de colisão, forças motrizes e esterçamento compatíveis com Rigidbody.
    /// </summary>
    [CreateAssetMenu(fileName = "New_AIVehicleData", menuName = "Projeto Grau 2/AI/Vehicle Data")]
    public class AIVehicleData : ScriptableObject
    {
        [Header("Classificação e Identificação")]
        public AIVehicleType vehicleType = AIVehicleType.EconomyCar;
        public string categoryName = "Carro Popular Hatch";

        [Header("Propriedades de Massa e Inércia")]
        [Tooltip("Massa total do veículo em quilogramas (kg).")]
        [Range(100f, 25000f)] public float mass = 1050f;

        [Tooltip("Arrasto aerodinâmico linear (Drag no Rigidbody).")]
        [Range(0.01f, 1.0f)] public float airDrag = 0.08f;

        [Tooltip("Arrasto angular para estabilização de guinada (Angular Drag no Rigidbody).")]
        [Range(0.05f, 5.0f)] public float angularDrag = 2.5f;

        [Header("Dimensões do Colisor Proxy (Metros)")]
        [Tooltip("Dimensões espaciais do BoxCollider: X = Largura, Y = Altura, Z = Comprimento.")]
        public Vector3 colliderSize = new Vector3(1.7f, 1.45f, 3.8f);

        [Tooltip("Deslocamento do centro do BoxCollider relativo ao pivô do veículo.")]
        public Vector3 colliderCenter = new Vector3(0f, 0.75f, 0f);

        [Tooltip("Entre-eixos (Wheelbase) em metros, para cálculo cinemático de esterçamento.")]
        [Range(1.2f, 12.0f)] public float wheelbase = 2.45f;

        [Header("Desempenho Longitudinal (Motor e Freios)")]
        [Tooltip("Velocidade máxima absoluta do veículo no plano (km/h).")]
        [Range(40f, 220f)] public float topSpeedKmh = 140f;

        [Tooltip("Força motriz de aceleração em Newtons (N).")]
        [Range(500f, 35000f)] public float accelerationForce = 3200f;

        [Tooltip("Força máxima de frenagem de serviço em Newtons (N).")]
        [Range(800f, 50000f)] public float brakeForce = 6500f;

        [Tooltip("Força motriz de marcha a ré em Newtons (N).")]
        [Range(300f, 15000f)] public float reverseForce = 1800f;

        [Tooltip("Velocidade máxima em marcha a ré (km/h).")]
        [Range(10f, 30f)] public float maxReverseSpeedKmh = 20f;

        [Header("Dinâmica de Esterçamento e Curvas")]
        [Tooltip("Ângulo máximo de esterçamento das rodas dianteiras (graus).")]
        [Range(15f, 45f)] public float maxSteerAngle = 35f;

        [Tooltip("Velocidade de transição e centralização do volante (graus/segundo).")]
        [Range(30f, 180f)] public float steerSpeed = 90f;

        [Tooltip("Fator de atenuação do esterçamento em altas velocidades para evitar capotamentos.")]
        [Range(0.2f, 1.0f)] public float highSpeedSteerScale = 0.5f;

        [Header("Comportamento de Motocicleta (Inclinação / Lean)")]
        [Tooltip("Indica se o veículo deve aplicar inclinação visual lateral em curvas (exclusivo para motos).")]
        public bool isTwoWheeled = false;

        [Tooltip("Ângulo máximo de inclinação corporal/chassi na curva (graus).")]
        [Range(0f, 45f)] public float maxLeanAngle = 30f;

        [Tooltip("Velocidade de retorno e transição de inclinação (graus/segundo).")]
        [Range(10f, 120f)] public float leanSpeed = 45f;

        [Header("Distâncias e Sensores Recomendados")]
        [Tooltip("Distância base de segurança frontal até o próximo veículo (metros).")]
        [Range(2.0f, 25.0f)] public float baseSafetyDistance = 6.0f;

        [Tooltip("Alcance do sensor frontal de longo alcance (metros).")]
        [Range(15.0f, 80.0f)] public float sensorForwardRange = 35.0f;
    }
}
