using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Controlador físico de frenagem e parada para veículos NPC.
    /// Atua em FixedUpdate desacelerando o Rigidbody contra seu vetor de velocidade.
    /// Impede efeito rebote (marcha ré involuntária) quando o veículo atinge repouso completo.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Brake Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class AIVehicleBrakeController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private AIVehicleData vehicleData;
        [SerializeField] private AIDriverProfile driverProfile;

        [Header("Entradas de Frenagem")]
        [Range(0f, 1f)] public float brakeInput = 0f;
        public bool isHandbrakeActive = false;

        private Rigidbody _rb;

        public bool IsBraking => brakeInput > 0.05f || isHandbrakeActive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Initialize(AIVehicleData data, AIDriverProfile profile)
        {
            vehicleData = data;
            driverProfile = profile;
        }

        private void FixedUpdate()
        {
            if (_rb == null || vehicleData == null) return;

            ApplyBrakingPhysics();
        }

        private void ApplyBrakingPhysics()
        {
            float speedMs = _rb.linearVelocity.magnitude;

            // Se o veículo estiver praticamente parado e freando, zera para evitar recuo
            if (speedMs < 0.25f && (brakeInput > 0.1f || isHandbrakeActive))
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                return;
            }

            if (speedMs > 0.1f && (brakeInput > 0.01f || isHandbrakeActive))
            {
                float multiplier = (driverProfile != null) ? driverProfile.brakeSmoothnessMultiplier : 1.0f;
                float finalBrakeForce = vehicleData.brakeForce * brakeInput * multiplier;

                if (isHandbrakeActive)
                {
                    finalBrakeForce = vehicleData.brakeForce * 1.5f;
                }

                Vector3 opposingDirection = -_rb.linearVelocity.normalized;
                _rb.AddForce(opposingDirection * finalBrakeForce, ForceMode.Force);
            }
        }

        public void SetBrake(float brake)
        {
            brakeInput = Mathf.Clamp01(brake);
        }

        public void SetEmergencyStop()
        {
            brakeInput = 1.0f;
            isHandbrakeActive = true;
        }

        public void ReleaseBrakes()
        {
            brakeInput = 0f;
            isHandbrakeActive = false;
        }
    }
}
