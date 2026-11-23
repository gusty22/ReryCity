using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Controlador físico de propulsão e motor para veículos NPC.
    /// Atua exclusivamente em FixedUpdate aplicando forças newtonianas ao Rigidbody.
    /// Suporta aceleração progressiva, controle proporcional de velocidade e marcha a ré.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Motor Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class AIVehicleMotorController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private AIVehicleData vehicleData;
        [SerializeField] private AIDriverProfile driverProfile;

        [Header("Entradas de Controle")]
        [Range(0f, 1f)] public float throttleInput = 0f;
        [Range(0f, 1f)] public float reverseThrottleInput = 0f;
        public float targetSpeedKmh = 60f;

        private Rigidbody _rb;
        private float _currentSpeedKmh = 0f;

        public float CurrentSpeedKmh => _currentSpeedKmh;
        public bool IsMovingForward => Vector3.Dot(_rb.linearVelocity, transform.forward) > 0.1f;
        public bool IsMovingBackward => Vector3.Dot(_rb.linearVelocity, transform.forward) < -0.1f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Initialize(AIVehicleData data, AIDriverProfile profile)
        {
            vehicleData = data;
            driverProfile = profile;

            if (_rb != null && vehicleData != null)
            {
                _rb.mass = vehicleData.mass;
                _rb.linearDamping = vehicleData.airDrag;
                _rb.angularDamping = vehicleData.angularDrag;
            }
        }

        private void FixedUpdate()
        {
            if (_rb == null || vehicleData == null) return;

            // Velocidade escalar no sentido longitudinal
            _currentSpeedKmh = Vector3.Dot(_rb.linearVelocity, transform.forward) * 3.6f;

            ApplyPropulsion();
        }

        private void ApplyPropulsion()
        {
            // Marcha à Ré
            if (reverseThrottleInput > 0.01f)
            {
                if (_currentSpeedKmh > -vehicleData.maxReverseSpeedKmh)
                {
                    float revForce = vehicleData.reverseForce * reverseThrottleInput;
                    _rb.AddForce(-transform.forward * revForce, ForceMode.Force);
                }
                return;
            }

            // Aceleração para Frente
            if (throttleInput > 0.01f)
            {
                float effectiveTopSpeed = Mathf.Min(targetSpeedKmh, vehicleData.topSpeedKmh);
                if (driverProfile != null)
                {
                    effectiveTopSpeed += driverProfile.speedOffsetKmh;
                }

                if (_currentSpeedKmh < effectiveTopSpeed)
                {
                    // Força proporcional à diferença de velocidade (com rampa suave)
                    float speedDiffRatio = Mathf.Clamp01((effectiveTopSpeed - _currentSpeedKmh) / 20f);
                    float accMultiplier = (driverProfile != null) ? driverProfile.accelerationMultiplier : 1.0f;
                    float finalForce = vehicleData.accelerationForce * throttleInput * speedDiffRatio * accMultiplier;

                    _rb.AddForce(transform.forward * finalForce, ForceMode.Force);
                }
            }
        }

        public void SetThrottle(float throttle)
        {
            throttleInput = Mathf.Clamp01(throttle);
            reverseThrottleInput = 0f;
        }

        public void SetReverseThrottle(float reverseThrottle)
        {
            reverseThrottleInput = Mathf.Clamp01(reverseThrottle);
            throttleInput = 0f;
        }

        public void StopThrottle()
        {
            throttleInput = 0f;
            reverseThrottleInput = 0f;
        }
    }
}
