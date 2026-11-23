using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;
using ProjetoGrau2.Core;
using ProjetoGrau2.Physics;

namespace ProjetoGrau2.Police
{
    public enum TrafficViolationType
    {
        Speeding,           // Excesso de velocidade
        WrongWay,           // Circulação na contramão
        RecklessWheelie,    // Manobra de grau / condução perigosa
        RedLightViolation,  // Desobediência à sinalização semafórica
        EvadingPolice       // Desobediência à ordem de parada / fuga
    }

    /// <summary>
    /// Detector e avaliador de infrações de trânsito cometidas pelo jogador.
    /// Monitora velocidade da moto, alinhamento vetorial com faixas, ângulo de grau e proximidade de semáforos vermelhos.
    /// Emprega temporizadores de cooldown e histerese para evitar duplicação ou alocação excessiva por frame.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Traffic Violation Detector")]
    public class TrafficViolationDetector : MonoBehaviour
    {
        public static TrafficViolationDetector Instance { get; private set; }

        [Header("Referências do Jogador")]
        [SerializeField] private MotorcycleController playerMotorcycle;
        [SerializeField] private WheeliePhysicsController playerWheelie;

        [Header("Tolerâncias e Limiares")]
        [Tooltip("Tolerância de velocidade acima do limite regulamentado antes de autuar (km/h).")]
        [Range(5f, 25f)] public float speedToleranceKmh = 12f;

        [Tooltip("Ângulo mínimo de inclinação frontal para considerar manobra de grau (graus).")]
        [Range(20f, 45f)] public float wheelieDetectionPitch = 25f;

        [Tooltip("Tempo contínuo de grau para caracterizar condução perigosa (segundos).")]
        [Range(0.5f, 3.0f)] public float wheelieDurationThreshold = 0.8f;

        [Header("Pesos das Infrações (Pontos de Procurado)")]
        public int scoreSpeeding = 15;
        public int scoreWrongWay = 25;
        public int scoreRecklessWheelie = 35;
        public int scoreRedLight = 40;
        public int scoreEvadingPolice = 60;

        [Header("Cooldowns de Infrações (Segundos)")]
        public float cooldownSpeeding = 4.0f;
        public float cooldownWrongWay = 3.5f;
        public float cooldownWheelie = 3.0f;
        public float cooldownRedLight = 6.0f;
        public float cooldownEvading = 5.0f;

        // Timers internos de cooldown
        private readonly Dictionary<TrafficViolationType, float> _cooldownTimers = new Dictionary<TrafficViolationType, float>();

        // Eventos
        public event Action<TrafficViolationType, int, string> OnViolationCommitted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeCooldowns();
            FindPlayerReferences();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializeCooldowns()
        {
            foreach (TrafficViolationType vType in Enum.GetValues(typeof(TrafficViolationType)))
            {
                _cooldownTimers[vType] = 0f;
            }
        }

        private void FindPlayerReferences()
        {
            if (playerMotorcycle == null)
            {
                playerMotorcycle = FindFirstObjectByType<MotorcycleController>();
            }
            if (playerWheelie == null && playerMotorcycle != null)
            {
                playerWheelie = playerMotorcycle.GetComponent<WheeliePhysicsController>();
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            UpdateCooldowns(dt);

            if (playerMotorcycle == null) return;

            EvaluateInfractions();
        }

        private void UpdateCooldowns(float dt)
        {
            var keys = new List<TrafficViolationType>(_cooldownTimers.Keys);
            foreach (var k in keys)
            {
                if (_cooldownTimers[k] > 0f)
                {
                    _cooldownTimers[k] -= dt;
                }
            }
        }

        private void EvaluateInfractions()
        {
            Vector3 playerPos = playerMotorcycle.transform.position;
            Vector3 playerForward = playerMotorcycle.transform.forward;
            float playerSpeed = playerMotorcycle.CurrentSpeedKmh;

            TrafficLane nearestLane = null;
            if (TrafficNavigationManager.Instance != null)
            {
                nearestLane = TrafficNavigationManager.Instance.GetNearestLane(playerPos, 15f);
            }

            // 1. Excesso de Velocidade
            if (nearestLane != null && playerSpeed > (nearestLane.speedLimitKmh + speedToleranceKmh))
            {
                TriggerViolation(TrafficViolationType.Speeding, scoreSpeeding,
                    $"Excesso de Velocidade! ({playerSpeed:F0} km/h em via de {nearestLane.speedLimitKmh:F0} km/h)",
                    cooldownSpeeding);
            }

            // 2. Circulação na Contramão
            if (nearestLane != null && playerSpeed > 15f)
            {
                var nearestWp = nearestLane.GetNearestWaypoint(playerPos);
                if (nearestWp != null)
                {
                    // Compara o vetor de avanço da moto com a direção do waypoint da faixa
                    float alignment = Vector3.Dot(playerForward, nearestWp.Forward);
                    if (alignment < -0.5f) // Ângulo maior que 120° em relação ao fluxo
                    {
                        TriggerViolation(TrafficViolationType.WrongWay, scoreWrongWay,
                            "Circulação na Contramão!",
                            cooldownWrongWay);
                    }
                }
            }

            // 3. Manobras de Grau em Via Pública
            if (playerWheelie != null && playerWheelie.CurrentPitchAngle >= wheelieDetectionPitch && playerSpeed > 10f)
            {
                if (playerWheelie.TimeInWheelie >= wheelieDurationThreshold)
                {
                    TriggerViolation(TrafficViolationType.RecklessWheelie, scoreRecklessWheelie,
                        $"Direção Perigosa! Grau em via pública ({playerWheelie.CurrentPitchAngle:F0}°)",
                        cooldownWheelie);
                }
            }

            // 4. Desobediência à Sinalização Semafórica (Furar Sinal Vermelho)
            if (nearestLane != null && nearestLane.LastWaypoint != null && nearestLane.LastWaypoint.isStopLine)
            {
                var stopWp = nearestLane.LastWaypoint;
                if (stopWp.AssociatedSignal != null && stopWp.AssociatedSignal.IsRed)
                {
                    float distToStop = Vector3.Distance(playerPos, stopWp.Position);
                    // Passou da linha de retenção com sinal fechado em velocidade
                    if (distToStop < 4.5f && playerSpeed > 15f)
                    {
                        TriggerViolation(TrafficViolationType.RedLightViolation, scoreRedLight,
                            "Avanço de Sinal Vermelho!",
                            cooldownRedLight);
                    }
                }
            }
        }

        public void ReportEvadingOrder()
        {
            TriggerViolation(TrafficViolationType.EvadingPolice, scoreEvadingPolice,
                "Desobediência à Ordem de Parada!",
                cooldownEvading);
        }

        private void TriggerViolation(TrafficViolationType type, int score, string description, float cooldownDuration)
        {
            if (_cooldownTimers[type] > 0f) return;

            _cooldownTimers[type] = cooldownDuration;
            OnViolationCommitted?.Invoke(type, score, description);

            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.AddViolationScore(score, description);
            }
        }
    }
}
