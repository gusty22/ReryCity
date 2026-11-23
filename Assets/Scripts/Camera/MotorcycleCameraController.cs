using UnityEngine;
using ProjetoGrau2.Inputs;
using ProjetoGrau2.Core;
using ProjetoGrau2.Physics;

namespace ProjetoGrau2.CameraControl
{
    public enum CameraViewMode
    {
        ThirdPersonChase,   // Câmera orbital e de perseguição clássica
        FirstPersonHelmet,  // Visão em primeira pessoa (no capacete do piloto)
        WheelieStuntAction  // Câmera dinâmica de ação para manobras de grau
    }

    /// <summary>
    /// Sistema de câmera veicular multiplataforma para PC e Android.
    /// Gerencia 3ª pessoa orbital com anti-clipping, 1ª pessoa no capacete com inclinação de curva,
    /// e câmera de ação para manobras de grau, totalmente desacoplado da simulação do Rigidbody.
    /// </summary>
    public class MotorcycleCameraController : MonoBehaviour
    {
        [Header("Alvo Principal")]
        [SerializeField] private Transform targetMotorcycle;
        [SerializeField] private MotorcycleController motorcycleController;
        [SerializeField] private WheeliePhysicsController wheelieController;

        [Header("Modo de Visão")]
        [SerializeField] private CameraViewMode currentMode = CameraViewMode.ThirdPersonChase;

        [Header("Configuração 3ª Pessoa (Chase)")]
        [SerializeField] private float thirdPersonDistance = 4.2f;
        [SerializeField] private float thirdPersonHeight = 1.6f;
        [SerializeField] private float thirdPersonLookHeight = 0.9f;
        [SerializeField] private LayerMask cameraCollisionMask = ~0;
        [SerializeField] private float collisionRadius = 0.25f;

        [Header("Configuração 1ª Pessoa (Capacete)")]
        [SerializeField] private Vector3 firstPersonOffset = new Vector3(0f, 1.25f, 0.15f);
        [SerializeField] private float leanBankingFactor = 0.65f; // Inclinação da cabeça nas curvas
        [SerializeField] private float firstPersonFov = 75f;

        [Header("Configuração Câmera de Ação (Stunt)")]
        [SerializeField] private float stuntDistance = 3.6f;
        [SerializeField] private float stuntSideOffset = 1.2f;
        [SerializeField] private float stuntHeight = 0.8f;

        [Header("Suavização")]
        [SerializeField] private float positionSmoothSpeed = 12f;
        [SerializeField] private float rotationSmoothSpeed = 10f;
        [SerializeField] private float autoCenterSpeed = 2.0f;

        // Referência de entrada
        private IMotorcycleInput inputProvider;
        private Camera targetCamera;
        private float defaultFov = 60f;

        // Ângulos orbitais livres
        private float orbitYaw = 0f;
        private float orbitPitch = 0f;
        private float lastLookTime = 0f;

        public CameraViewMode CurrentMode => currentMode;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
            if (targetCamera != null) defaultFov = targetCamera.fieldOfView;

            if (targetMotorcycle != null)
            {
                if (motorcycleController == null) motorcycleController = targetMotorcycle.GetComponent<MotorcycleController>();
                if (wheelieController == null) wheelieController = targetMotorcycle.GetComponent<WheeliePhysicsController>();
                inputProvider = targetMotorcycle.GetComponent<IMotorcycleInput>() ?? targetMotorcycle.GetComponentInChildren<IMotorcycleInput>();
            }
        }

        private void LateUpdate()
        {
            if (targetMotorcycle == null) return;

            // 1. Processamento de alternância de câmera
            if (inputProvider != null && inputProvider.CameraCycleTriggered)
            {
                CycleNextMode();
            }

            // 2. Processamento do LookDelta (órbita com mouse ou touch)
            HandleLookInput();

            // 3. Atualização de perspectiva conforme o modo
            switch (currentMode)
            {
                case CameraViewMode.ThirdPersonChase:
                    UpdateThirdPersonChase();
                    break;
                case CameraViewMode.FirstPersonHelmet:
                    UpdateFirstPersonHelmet();
                    break;
                case CameraViewMode.WheelieStuntAction:
                    UpdateWheelieStuntAction();
                    break;
            }
        }

        public void CycleNextMode()
        {
            currentMode = (CameraViewMode)(((int)currentMode + 1) % 3);
        }

        public void SetMode(CameraViewMode mode)
        {
            currentMode = mode;
        }

        private void HandleLookInput()
        {
            if (inputProvider == null) return;

            Vector2 look = inputProvider.LookDelta;
            if (look.sqrMagnitude > 0.01f)
            {
                orbitYaw += look.x;
                orbitPitch = Mathf.Clamp(orbitPitch - look.y, -30f, 60f);
                lastLookTime = Time.time;
            }
            else
            {
                // Auto-recentra suavemente a câmera atrás da moto após 1.5s sem interação manual
                if (Time.time - lastLookTime > 1.5f && currentMode != CameraViewMode.FirstPersonHelmet)
                {
                    orbitYaw = Mathf.Lerp(orbitYaw, 0f, Time.deltaTime * autoCenterSpeed);
                    orbitPitch = Mathf.Lerp(orbitPitch, 0f, Time.deltaTime * autoCenterSpeed);
                }
            }
        }

        private void UpdateThirdPersonChase()
        {
            if (targetCamera != null && targetCamera.fieldOfView != defaultFov)
            {
                targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, defaultFov, Time.deltaTime * 6f);
            }

            // Rotação horizontal baseada no Yaw da moto + órbita manual
            float targetYaw = targetMotorcycle.eulerAngles.y + orbitYaw;
            Quaternion orbitRotation = Quaternion.Euler(orbitPitch, targetYaw, 0f);

            // Ponto de foco na moto
            float wheeliePitch = wheelieController != null ? wheelieController.CurrentPitchAngle : 0f;
            Vector3 lookTarget = targetMotorcycle.position + Vector3.up * (thirdPersonLookHeight + (wheeliePitch * 0.008f));

            // Posição desejada sem colisões
            Vector3 desiredOffset = orbitRotation * (Vector3.back * thirdPersonDistance) + Vector3.up * thirdPersonHeight;
            Vector3 targetPosition = targetMotorcycle.position + desiredOffset;

            // Anti-clipping de colisão com cenário (SphereCast da moto até a câmera)
            Vector3 direction = (targetPosition - lookTarget).normalized;
            float maxDist = Vector3.Distance(lookTarget, targetPosition);
            if (Physics.SphereCast(lookTarget, collisionRadius, direction, out RaycastHit hit, maxDist, cameraCollisionMask))
            {
                targetPosition = hit.point + hit.normal * collisionRadius;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

            // Orientação para o ponto de foco
            Vector3 finalLookDir = (lookTarget - transform.position).normalized;
            if (finalLookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalLookDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
            }
        }

        private void UpdateFirstPersonHelmet()
        {
            if (targetCamera != null && targetCamera.fieldOfView != firstPersonFov)
            {
                targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, firstPersonFov, Time.deltaTime * 8f);
            }

            // Posição ancorada rigidamente na cabeça do piloto
            Vector3 helmetPosition = targetMotorcycle.TransformPoint(firstPersonOffset);
            transform.position = helmetPosition;

            // Rotação: acompanha a moto com inclinação lateral nas curvas (lean banking)
            Vector3 targetEuler = targetMotorcycle.eulerAngles;
            float roll = targetEuler.z;
            if (roll > 180f) roll -= 360f;

            // A cabeça do motociclista compensa parte da gravidade nas curvas
            float compensatedRoll = roll * leanBankingFactor;

            Quaternion helmetRotation = Quaternion.Euler(targetEuler.x + orbitPitch, targetEuler.y + orbitYaw, compensatedRoll);
            transform.rotation = Quaternion.Slerp(transform.rotation, helmetRotation, Time.deltaTime * 25f);
        }

        private void UpdateWheelieStuntAction()
        {
            if (targetCamera != null && targetCamera.fieldOfView != defaultFov + 5f)
            {
                targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, defaultFov + 5f, Time.deltaTime * 6f);
            }

            // Câmera lateral/diagonal dramática
            float targetYaw = targetMotorcycle.eulerAngles.y - 25f + orbitYaw;
            Quaternion stuntRot = Quaternion.Euler(12f, targetYaw, 0f);

            Vector3 stuntOffset = (stuntRot * Vector3.back * stuntDistance) + (targetMotorcycle.right * stuntSideOffset) + Vector3.up * stuntHeight;
            Vector3 targetPosition = targetMotorcycle.position + stuntOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

            // Olha para a região entre o motor e a roda dianteira em ascensão
            Vector3 focalPoint = targetMotorcycle.position + targetMotorcycle.forward * 0.4f + Vector3.up * 1.1f;
            Vector3 lookDir = (focalPoint - transform.position).normalized;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.deltaTime * rotationSmoothSpeed);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            targetMotorcycle = newTarget;
            if (targetMotorcycle != null)
            {
                motorcycleController = targetMotorcycle.GetComponent<MotorcycleController>();
                wheelieController = targetMotorcycle.GetComponent<WheeliePhysicsController>();
                inputProvider = targetMotorcycle.GetComponent<IMotorcycleInput>() ?? targetMotorcycle.GetComponentInChildren<IMotorcycleInput>();
            }
        }
    }
}
