using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Inputs;
using ProjetoGrau2.Physics;
using ProjetoGrau2.Propulsion;

namespace ProjetoGrau2.Core
{
    /// <summary>
    /// Controlador principal da dinâmica veicular, integrando Rigidbody,
    /// propulsão física real (Engine + Transmission + AthlonECU),
    /// controle do grau (WheeliePhysicsController) e entrada (IMotorcycleInput).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MotorcycleController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private MotorcycleData data;

        [Header("Subsistemas Físicos")]
        [SerializeField] private WheeliePhysicsController wheelieController;

        [Header("Subsistemas de Propulsão Real")]
        [SerializeField] private EngineController engineController;
        [SerializeField] private TransmissionController transmissionController;
        [SerializeField] private AthlonECUController ecuController;

        [Header("Pontos de Contato / Rodas")]
        [SerializeField] private Transform frontWheelTransform;
        [SerializeField] private Transform rearWheelTransform;
        [SerializeField] private float wheelRadius = 0.32f;
        [SerializeField] private LayerMask groundLayer = ~0;

        [Header("Propriedades Dinâmicas")]
        [SerializeField] private float topSpeedKmh = 140f;
        [SerializeField] private float fallbackAccelerationForce = 1800f;
        [SerializeField] private float brakeForce = 2500f;
        [SerializeField] private float steeringTorque = 450f;
        [SerializeField] private float downforceCoefficient = 12f;

        // Referências internas
        private Rigidbody rb;
        private IMotorcycleInput inputProvider;

        // Estados em tempo real
        private bool isFrontGrounded;
        private bool isRearGrounded;
        private float currentSpeedKmh;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;

        public MotorcycleData Data => data;
        public Rigidbody Rigidbody => rb;
        public WheeliePhysicsController WheelieController => wheelieController;
        public EngineController Engine => engineController;
        public TransmissionController Transmission => transmissionController;
        public AthlonECUController ECU => ecuController;
        public bool IsFrontGrounded => isFrontGrounded;
        public bool IsRearGrounded => isRearGrounded;
        public float CurrentSpeedKmh => currentSpeedKmh;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (wheelieController == null) wheelieController = GetComponent<WheeliePhysicsController>();
            if (engineController == null) engineController = GetComponent<EngineController>();
            if (transmissionController == null) transmissionController = GetComponent<TransmissionController>();
            if (ecuController == null) ecuController = GetComponent<AthlonECUController>();

            inputProvider = GetComponent<IMotorcycleInput>() ?? GetComponentInChildren<IMotorcycleInput>();

            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            ConfigureRigidbody();
        }

        private void Start()
        {
            if (data != null && wheelieController != null && inputProvider != null)
            {
                wheelieController.Initialize(data, inputProvider);
            }
        }

        public void SetInputProvider(IMotorcycleInput provider)
        {
            inputProvider = provider;
            if (wheelieController != null && data != null)
            {
                wheelieController.Initialize(data, inputProvider);
            }
        }

        public void SetData(MotorcycleData newData)
        {
            data = newData;
            ConfigureRigidbody();
            if (wheelieController != null && inputProvider != null)
            {
                wheelieController.Initialize(data, inputProvider);
            }
        }

        private void ConfigureRigidbody()
        {
            if (data != null && rb != null)
            {
                rb.mass = data.mass;
                rb.centerOfMass = data.centerOfMassOffset;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        private void FixedUpdate()
        {
            if (inputProvider == null) return;

            UpdateGroundStatus();
            currentSpeedKmh = Vector3.Dot(rb.linearVelocity, transform.forward) * 3.6f;

            if (inputProvider.ResetTriggered || (wheelieController != null && wheelieController.IsCrashed && inputProvider.ResetTriggered))
            {
                ResetMotorcycle();
                return;
            }

            // Mudança de marchas manual/assistida
            HandleGearShifting();

            ApplyPropulsionAndEngine();
            ApplySteering();
            ApplyDownforceAndStabilization();
        }

        private void HandleGearShifting()
        {
            if (transmissionController == null || inputProvider == null) return;

            if (inputProvider.ShiftUpTriggered)
            {
                transmissionController.ShiftUp();
            }
            else if (inputProvider.ShiftDownTriggered)
            {
                transmissionController.ShiftDown();
            }
        }

        private void UpdateGroundStatus()
        {
            Vector3 frontOrigin = frontWheelTransform != null ? frontWheelTransform.position : transform.position + transform.forward * 0.7f;
            Vector3 rearOrigin = rearWheelTransform != null ? rearWheelTransform.position : transform.position - transform.forward * 0.7f;

            // Raio de solo vertical imune ao pitch do grau
            isFrontGrounded = UnityEngine.Physics.Raycast(frontOrigin, Vector3.down, wheelRadius + 0.08f, groundLayer);
            isRearGrounded = UnityEngine.Physics.Raycast(rearOrigin, Vector3.down, wheelRadius + 0.08f, groundLayer);
        }

        private void ApplyPropulsionAndEngine()
        {
            float throttle = inputProvider.Throttle;
            float rearBrake = inputProvider.RearBrake;
            float frontBrake = inputProvider.FrontBrake;
            float clutch = inputProvider.Clutch;
            float linearSpeedMs = Vector3.Dot(rb.linearVelocity, transform.forward);

            // 1. Simulação do Trem de Força Físico Real (Motor + Câmbio + Athlon ECU)
            if (engineController != null && transmissionController != null)
            {
                // A) Avalia corte de ignição/two-step da ECU Athlon
                bool isEcuCut = (ecuController != null) && ecuController.EvaluateCut(
                    engineController.CurrentRpm,
                    Mathf.Abs(currentSpeedKmh),
                    throttle,
                    transmissionController.ClutchEngagement
                );

                // B) Transmissão calcula o RPM imposto pela roda e o torque de saída
                float equivalentRpm = transmissionController.SimulateTransmission(
                    engineController.CurrentTorqueNm,
                    linearSpeedMs,
                    wheelRadius,
                    clutch,
                    Time.fixedDeltaTime
                );

                // C) Motor calcula termodinâmica, combustão e novo RPM
                engineController.SimulatePhysics(
                    throttle,
                    equivalentRpm,
                    transmissionController.ClutchEngagement,
                    isEcuCut,
                    Time.fixedDeltaTime
                );

                // D) Aplicação da força tratora física na roda traseira
                if (isRearGrounded && currentSpeedKmh < topSpeedKmh)
                {
                    Vector3 tractiveForceVector = transform.forward * transmissionController.TractiveForceN;
                    rb.AddForce(tractiveForceVector, ForceMode.Force);
                }
            }
            else
            {
                // Modo Fallback simplificado (quando os módulos de propulsão não estiverem anexados)
                if (isRearGrounded && throttle > 0.01f && currentSpeedKmh < topSpeedKmh)
                {
                    float speedFactor = Mathf.Clamp01(1f - (currentSpeedKmh / topSpeedKmh));
                    Vector3 force = transform.forward * (throttle * fallbackAccelerationForce * speedFactor);
                    rb.AddForce(force, ForceMode.Force);
                }
            }

            // 2. Frenagem Mecânica
            if (rearBrake > 0.01f)
            {
                Vector3 brakeVector = -rb.linearVelocity.normalized * (rearBrake * brakeForce * 0.45f);
                rb.AddForce(brakeVector, ForceMode.Force);
            }

            if (frontBrake > 0.01f && isFrontGrounded)
            {
                Vector3 brakeVector = -rb.linearVelocity.normalized * (frontBrake * brakeForce * 0.75f);
                rb.AddForce(brakeVector, ForceMode.Force);
            }
        }

        private void ApplySteering()
        {
            float steerInput = inputProvider.Steering;
            if (Mathf.Abs(steerInput) < 0.01f) return;

            // Restrição de esterçamento em grau elevado (noTurnAngle)
            if (wheelieController != null && data != null)
            {
                if (wheelieController.CurrentPitchAngle > data.noTurnAngle)
                {
                    steerInput = 0f;
                }
            }

            if (Mathf.Abs(steerInput) > 0.01f && isRearGrounded)
            {
                float steerFactor = Mathf.Clamp01(Mathf.Abs(currentSpeedKmh) / 20f);
                Vector3 yawTorque = transform.up * (steerInput * steeringTorque * steerFactor);
                rb.AddTorque(yawTorque, ForceMode.Force);

                Vector3 rollTorque = -transform.forward * (steerInput * (steeringTorque * 0.25f) * steerFactor);
                rb.AddTorque(rollTorque, ForceMode.Force);
            }
        }

        private void ApplyDownforceAndStabilization()
        {
            if (currentSpeedKmh > 10f)
            {
                float downforce = downforceCoefficient * (currentSpeedKmh / 10f);
                rb.AddForce(-transform.up * downforce, ForceMode.Force);
            }

            Vector3 lateralVelocity = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
            rb.AddForce(-lateralVelocity * (rb.mass * 12f), ForceMode.Force);
        }

        public void ResetMotorcycle()
        {
            Vector3 resetPos = spawnPosition + Vector3.up * 0.5f;
            Quaternion resetRot = Quaternion.Euler(0f, spawnRotation.eulerAngles.y, 0f);

            transform.position = resetPos;
            transform.rotation = resetRot;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (wheelieController != null)
            {
                wheelieController.ResetPhysics(resetPos, resetRot);
            }

            if (engineController != null)
            {
                engineController.StartEngine();
            }

            if (transmissionController != null)
            {
                transmissionController.SetGear(1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isRearGrounded ? Color.green : Color.red;
            Vector3 rearOrigin = rearWheelTransform != null ? rearWheelTransform.position : transform.position - transform.forward * 0.7f;
            Gizmos.DrawWireSphere(rearOrigin, wheelRadius);

            Gizmos.color = isFrontGrounded ? Color.green : Color.blue;
            Vector3 frontOrigin = frontWheelTransform != null ? frontWheelTransform.position : transform.position + transform.forward * 0.7f;
            Gizmos.DrawWireSphere(frontOrigin, wheelRadius);
        }
    }
}
