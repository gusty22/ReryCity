using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Controlador físico e cinemático de direção e esterçamento para veículos NPC.
    /// Calcula o ângulo de esterçamento via Pure Pursuit / Stanley em direção ao waypoint alvo,
    /// aplica torques de alinhamento no Rigidbody e controla a inclinação visual (lean banking) para motocicletas.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Steering Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class AIVehicleSteeringController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private AIVehicleData vehicleData;

        [Header("Alvo de Direção")]
        [SerializeField] private Transform targetWaypointTransform;
        [SerializeField] private Vector3 targetWorldPosition;

        [Header("Visual e Inclinação (Motocicletas)")]
        [Tooltip("Transform do chassi/malha visual que sofrerá inclinação lateral em curvas (apenas moto).")]
        [SerializeField] private Transform visualChassisTransform;

        [Header("Entradas e Estado Atual")]
        [Range(-1f, 1f)] public float steerInput = 0f;
        public float currentSteerAngle = 0f;
        public float currentLeanAngle = 0f;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Initialize(AIVehicleData data, Transform visualMesh = null)
        {
            vehicleData = data;
            if (visualMesh != null)
            {
                visualChassisTransform = visualMesh;
            }
        }

        public void SetTargetPosition(Vector3 worldPos)
        {
            targetWorldPosition = worldPos;
            targetWaypointTransform = null;
        }

        public void SetTargetWaypoint(Transform wp)
        {
            targetWaypointTransform = wp;
            if (wp != null)
            {
                targetWorldPosition = wp.position;
            }
        }

        private void FixedUpdate()
        {
            if (_rb == null || vehicleData == null) return;

            UpdateSteerInput();
            ApplySteeringPhysics();
            ApplyLateralFriction();
            ApplyVisualLean();
        }

        private void UpdateSteerInput()
        {
            if (targetWaypointTransform != null)
            {
                targetWorldPosition = targetWaypointTransform.position;
            }

            Vector3 localTarget = transform.InverseTransformPoint(targetWorldPosition);

            // Se o alvo estiver à frente, calcula ângulo desejado
            if (localTarget.z > 0.1f)
            {
                float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
                float clampedTargetAngle = Mathf.Clamp(targetAngle, -vehicleData.maxSteerAngle, vehicleData.maxSteerAngle);

                // Atenuação em alta velocidade para estabilidade
                float speedKmh = _rb.linearVelocity.magnitude * 3.6f;
                float speedFactor = Mathf.Lerp(1.0f, vehicleData.highSpeedSteerScale, Mathf.Clamp01(speedKmh / 100f));
                clampedTargetAngle *= speedFactor;

                // Transição suave
                currentSteerAngle = Mathf.MoveTowards(
                    currentSteerAngle,
                    clampedTargetAngle,
                    vehicleData.steerSpeed * Time.fixedDeltaTime
                );

                steerInput = Mathf.Clamp(currentSteerAngle / vehicleData.maxSteerAngle, -1f, 1f);
            }
            else
            {
                steerInput = Mathf.MoveTowards(steerInput, 0f, 2f * Time.fixedDeltaTime);
                currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, 0f, vehicleData.steerSpeed * Time.fixedDeltaTime);
            }
        }

        private void ApplySteeringPhysics()
        {
            float forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);

            // Só gira se houver deslocamento
            if (Mathf.Abs(forwardSpeed) > 0.5f)
            {
                float directionSign = Mathf.Sign(forwardSpeed);
                float turnTorque = steerInput * directionSign * (vehicleData.mass * 8.0f);

                _rb.AddTorque(Vector3.up * turnTorque, ForceMode.Force);
            }
        }

        private void ApplyLateralFriction()
        {
            // Amortecimento de derrapagem lateral (adesão dos pneus ao asfalto)
            Vector3 lateralVelocity = Vector3.Dot(_rb.linearVelocity, transform.right) * transform.right;
            _rb.AddForce(-lateralVelocity * (vehicleData.mass * 12.0f), ForceMode.Force);
        }

        private void ApplyVisualLean()
        {
            if (!vehicleData.isTwoWheeled || visualChassisTransform == null) return;

            float speedRatio = Mathf.Clamp01(_rb.linearVelocity.magnitude / 20f);
            float targetLean = -steerInput * vehicleData.maxLeanAngle * speedRatio;

            currentLeanAngle = Mathf.MoveTowards(
                currentLeanAngle,
                targetLean,
                vehicleData.leanSpeed * Time.fixedDeltaTime
            );

            visualChassisTransform.localRotation = Quaternion.Euler(0f, 0f, currentLeanAngle);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, targetWorldPosition + Vector3.up * 0.5f);
            Gizmos.DrawWireSphere(targetWorldPosition + Vector3.up * 0.5f, 0.4f);
        }
    }
}
