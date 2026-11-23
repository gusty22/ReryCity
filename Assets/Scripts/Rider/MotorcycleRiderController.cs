using System;
using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Core;
using ProjetoGrau2.Physics;
using ProjetoGrau2.Inputs;

namespace ProjetoGrau2.Rider
{
    public enum RiderState
    {
        Idle,               // Parado sobre a moto
        Riding,             // Pilotagem normal e curvas
        Accelerating,       // Tronco projetado para frente sob aceleração
        Braking,            // Tronco empurrado contra o guidão sob frenagem
        WheelieAscending,   // Levantando a roda dianteira
        WheelieBalanced,    // Grau estabilizado no ponto de equilíbrio
        WheelieRaspa,       // Postura extrema de raspão (corpo esticado para trás)
        CrashedRagdoll      // Queda física desacoplada
    }

    /// <summary>
    /// Orquestrador principal do piloto humanoide.
    /// Gerencia estados corporais, integra animações com a física da moto e
    /// comanda a transição para Ragdoll em capotamentos e colisões.
    /// </summary>
    public class MotorcycleRiderController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private RiderData data;

        [Header("Referências da Motocicleta")]
        [SerializeField] private MotorcycleController motorcycleController;
        [SerializeField] private WheeliePhysicsController wheelieController;

        [Header("Subsistemas do Piloto")]
        [SerializeField] private RiderIKController ikController;
        [SerializeField] private RiderPoseController poseController;
        [SerializeField] private RiderRagdollController ragdollController;

        // Estados em tempo de execução
        private RiderState currentState = RiderState.Idle;
        private IMotorcycleInput inputProvider;

        // Eventos públicos
        public event Action<RiderState> OnRiderStateChanged;
        public event Action OnRiderCrashed;
        public event Action OnRiderRecovered;

        public RiderData Data => data;
        public RiderState CurrentState => currentState;
        public MotorcycleController Motorcycle => motorcycleController;

        public void Initialize(RiderData riderData, MotorcycleController motorcycle)
        {
            data = riderData;
            motorcycleController = motorcycle;
            if (motorcycleController != null)
            {
                wheelieController = motorcycleController.WheelieController;
                inputProvider = motorcycleController.GetComponent<IMotorcycleInput>() ?? motorcycleController.GetComponentInChildren<IMotorcycleInput>();
            }

            if (ikController != null) ikController.Initialize(data, motorcycleController);
            if (poseController != null) poseController.Initialize(data, motorcycleController, wheelieController);
            if (ragdollController != null) ragdollController.Initialize(this);

            RegisterEvents();
        }

        private void Awake()
        {
            if (motorcycleController == null)
            {
                motorcycleController = GetComponentInParent<MotorcycleController>();
            }

            if (ikController == null) ikController = GetComponent<RiderIKController>();
            if (poseController == null) poseController = GetComponent<RiderPoseController>();
            if (ragdollController == null) ragdollController = GetComponent<RiderRagdollController>();

            if (motorcycleController != null)
            {
                wheelieController = motorcycleController.WheelieController;
                inputProvider = motorcycleController.GetComponent<IMotorcycleInput>() ?? motorcycleController.GetComponentInChildren<IMotorcycleInput>();
            }
        }

        private void Start()
        {
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (wheelieController != null)
            {
                wheelieController.OnCrashTriggered += HandleCrash;
            }
        }

        private void UnregisterEvents()
        {
            if (wheelieController != null)
            {
                wheelieController.OnCrashTriggered -= HandleCrash;
            }
        }

        private void Update()
        {
            if (currentState == RiderState.CrashedRagdoll)
            {
                // Verifica tecla de recuperação se necessário
                if (inputProvider != null && inputProvider.ResetTriggered)
                {
                    RecoverRider();
                }
                return;
            }

            UpdateRiderState();
        }

        private void UpdateRiderState()
        {
            if (motorcycleController == null) return;

            float speedKmh = motorcycleController.CurrentSpeedKmh;
            float pitchAngle = wheelieController != null ? wheelieController.CurrentPitchAngle : 0f;
            float throttle = inputProvider != null ? inputProvider.Throttle : 0f;
            float rearBrake = inputProvider != null ? inputProvider.RearBrake : 0f;

            RiderState newState = RiderState.Idle;

            // 1. Manobras de Grau
            if (pitchAngle > 18f)
            {
                if (pitchAngle >= 72f)
                {
                    newState = RiderState.WheelieRaspa; // Corpo esticado e postura de raspão
                }
                else if (pitchAngle >= 45f)
                {
                    newState = RiderState.WheelieBalanced;
                }
                else
                {
                    newState = RiderState.WheelieAscending;
                }
            }
            // 2. Pilotagem em Solo
            else if (Mathf.Abs(speedKmh) > 2f)
            {
                if (rearBrake > 0.4f || (inputProvider != null && inputProvider.FrontBrake > 0.4f))
                {
                    newState = RiderState.Braking;
                }
                else if (throttle > 0.6f)
                {
                    newState = RiderState.Accelerating;
                }
                else
                {
                    newState = RiderState.Riding;
                }
            }
            else
            {
                newState = RiderState.Idle;
            }

            if (newState != currentState)
            {
                currentState = newState;
                OnRiderStateChanged?.Invoke(currentState);
            }
        }

        public void HandleCrash()
        {
            if (currentState == RiderState.CrashedRagdoll) return;

            currentState = RiderState.CrashedRagdoll;
            OnRiderStateChanged?.Invoke(currentState);
            OnRiderCrashed?.Invoke();

            // Ativa Ragdoll físico desacoplando do Animator
            if (ragdollController != null)
            {
                ragdollController.ActivateRagdoll();
            }
        }

        public void RecoverRider()
        {
            if (currentState != RiderState.CrashedRagdoll) return;

            if (ragdollController != null)
            {
                ragdollController.DeactivateRagdoll();
            }

            currentState = RiderState.Idle;
            OnRiderStateChanged?.Invoke(currentState);
            OnRiderRecovered?.Invoke();

            if (motorcycleController != null)
            {
                motorcycleController.ResetMotorcycle();
            }
        }
    }
}
