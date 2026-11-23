using System;
using UnityEngine;

namespace ProjetoGrau2.Data
{
    /// <summary>
    /// Definição de dados físicos e comportamentais da motocicleta via ScriptableObject.
    /// Segue a arquitetura ADR-003, permitindo calibração visual e modular no Unity Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMotorcycleData", menuName = "Projeto Grau 2/Física/Motorcycle Data")]
    public class MotorcycleData : ScriptableObject
    {
        [Header("Identificação do Modelo")]
        [Tooltip("Nome de exibição da motocicleta.")]
        public string motorcycleName = "CG 160 Titan (Provisória)";

        [Tooltip("Categoria do veículo (ex: Street, BigTrail, Naked, Motoneta).")]
        public string category = "Street";

        [Header("Propriedades de Massa e Inércia (Rigidbody)")]
        [Tooltip("Massa total da motocicleta em quilogramas (ex: 120kg para 160cc).")]
        [Range(70f, 300f)] public float mass = 125f;

        [Tooltip("Deslocamento do centro de massa em relação ao pivô geométrico da malha (X, Y, Z).")]
        public Vector3 centerOfMassOffset = new Vector3(0f, -0.15f, 0.05f);

        [Tooltip("Multiplicador do tensor de inércia para controle de estabilidade giroscópica.")]
        public Vector3 inertiaTensorMultiplier = new Vector3(1.1f, 1.0f, 1.2f);

        [Header("Geometria e Limiares Angulares do Grau (Pitch)")]
        [Tooltip("Ângulo em graus do ponto de equilíbrio dinâmico (alinhamento com eixo traseiro). Comprovado no BikeWheelie.")]
        [Range(45f, 75f)] public float balanceAngle = 60f;

        [Tooltip("Faixa angular (em graus) de desaceleração suave antes do ponto de equilíbrio.")]
        [Range(5f, 25f)] public float liftEaseRange = 15f;

        [Tooltip("Ângulo em graus onde o esterçamento do guidão perde autoridade lateral no grau.")]
        [Range(40f, 70f)] public float noTurnAngle = 52f;

        [Tooltip("Ângulo crítico em graus no qual a moto vira para trás e ejeta o piloto.")]
        [Range(75f, 90f)] public float crashAngle = 83f;

        [Header("Torques Físicos de Manobra (Nm / Força Angular)")]
        [Tooltip("Torque rotacional de subida aplicado no eixo traseiro para erguer a frente.")]
        [Range(100f, 1000f)] public float liftTorque = 420f;

        [Tooltip("Torque corretivo aplicado pelo pedal de freio traseiro para puxar a dianteira para baixo.")]
        [Range(150f, 1200f)] public float brakeTorque = 550f;

        [Tooltip("Fator multiplicador do torque restaurador da gravidade.")]
        [Range(0.5f, 2.5f)] public float gravityTorqueFactor = 1.0f;

        [Tooltip("Amortecimento angular contra oscilações e instabilidade de arfagem (Damping).")]
        [Range(0.1f, 10f)] public float damping = 2.5f;

        [Header("Assistências de Pilotagem e Auto-Sustentação")]
        [Tooltip("Intensidade da assistência de estabilização em torno do ponto de equilíbrio (0 = física pura, 1 = arcade).")]
        [Range(0f, 1f)] public float balanceAssist = 0.45f;

        [Tooltip("Janela angular em torno de balanceAngle onde a assistência atua com eficácia máxima.")]
        [Range(2f, 15f)] public float balanceAssistRange = 6f;

        [Tooltip("Fator de auto-sustentação da moto no grau quase sem acelerador quando em equilíbrio.")]
        [Range(0f, 1f)] public float selfSustainBlend = 0.35f;

        [Header("Resposta de Aceleração e Retorno ao Solo")]
        [Tooltip("Sensibilidade e rampa de ganho do torque de subida pelo acelerador.")]
        [Range(0.5f, 2.0f)] public float throttleLiftFactor = 1.15f;

        [Tooltip("Tempo em segundos para transição suave de aceleração (filtro de entrada).")]
        [Range(0.05f, 0.5f)] public float accelSmoothing = 0.12f;

        [Tooltip("Velocidade angular com que a frente retorna ao solo ao soltar o grau.")]
        [Range(1f, 10f)] public float recoverySpeed = 4.5f;

        [Tooltip("Suavização do amortecimento ao tocar a roda dianteira no asfalto.")]
        [Range(0.1f, 2f)] public float recoveryBlend = 0.8f;

        [Header("Dinâmica Lateral e Contra-Esterço")]
        [Tooltip("Ativa contra-esterço dinâmico na puxada inicial.")]
        public bool counterSteer = true;

        [Tooltip("Graus de contra-esterço aplicados por cada grau de inclinação lateral.")]
        [Range(0f, 1.5f)] public float counterSteerPerLeanDegree = 0.4f;

        [Tooltip("Permite fazer curvas suaves enquanto empina sem desestabilizar.")]
        public bool curvaSuaveNoGrau = true;
    }
}
