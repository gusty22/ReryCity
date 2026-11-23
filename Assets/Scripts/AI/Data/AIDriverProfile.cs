using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    public enum DriverPersonality
    {
        Cautious,   // Cauteloso: distância ampla, freia antes, respeita estritamente semáforos
        Normal,     // Convencional: aceleração/frenagem progressivas, ultrapassagens seguras
        Aggressive  // Agressivo: aceleração rápida, menor distância segura, mudanças frequentes de faixa
    }

    /// <summary>
    /// Perfil psicológico e comportamental de direção do condutor NPC.
    /// Define fatores de reação, tolerância de velocidade, distâncias de seguimento e agressividade.
    /// </summary>
    [CreateAssetMenu(fileName = "New_DriverProfile", menuName = "Projeto Grau 2/AI/Driver Profile")]
    public class AIDriverProfile : ScriptableObject
    {
        [Header("Personalidade do Condutor")]
        public DriverPersonality personality = DriverPersonality.Normal;
        public string profileName = "Motorista Convencional";

        [Header("Distâncias e Margens de Segurança")]
        [Tooltip("Multiplicador da distância base de segurança do veículo.")]
        [Range(0.6f, 2.5f)] public float safeDistanceMultiplier = 1.0f;

        [Tooltip("Distância extra mínima de parada antes de obstáculos e semáforos (metros).")]
        [Range(1.0f, 6.0f)] public float stopLineMargin = 2.5f;

        [Header("Dinâmica de Condução")]
        [Tooltip("Multiplicador da força de aceleração base.")]
        [Range(0.6f, 1.5f)] public float accelerationMultiplier = 1.0f;

        [Tooltip("Multiplicador da potência de frenagem normal.")]
        [Range(0.7f, 1.5f)] public float brakeSmoothnessMultiplier = 1.0f;

        [Tooltip("Tolerância sobre o limite de velocidade regulamentado da via (km/h). Ex: -5 km/h (lento), +10 km/h (apressado).")]
        [Range(-15f, 25f)] public float speedOffsetKmh = 0f;

        [Header("Tomada de Decisão e Ultrapassagem")]
        [Tooltip("Tempo de reação/histerese do condutor antes de decidir mudar de estado (segundos).")]
        [Range(0.15f, 1.2f)] public float reactionTime = 0.45f;

        [Tooltip("Probabilidade de aceitar oportunidade de ultrapassagem ou mudança de faixa quando livre (0 a 1).")]
        [Range(0.0f, 1.0f)] public float laneChangePropensity = 0.35f;

        [Tooltip("Tempo mínimo de permanência em uma faixa antes de avaliar nova mudança (segundos).")]
        [Range(2.0f, 15.0f)] public float minTimeBetweenLaneChanges = 5.0f;

        [Tooltip("Taxa de conformidade semafórica (1.0 = nunca avança no vermelho, 0.95 = chance rara de arrancar no amarelo tardio).")]
        [Range(0.9f, 1.0f)] public float signalCompliance = 1.0f;
    }
}
