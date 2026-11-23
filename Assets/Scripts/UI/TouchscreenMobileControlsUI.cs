using System;
using UnityEngine;
using UnityEngine.UI;
using ProjetoGrau2.Inputs;

namespace ProjetoGrau2.UI
{
    /// <summary>
    /// Painel de controles virtuais para telas sensíveis ao toque (Touchscreen Mobile Controls).
    /// Conecta botões táteis de tela diretamente ao CrossPlatformInputHandler,
    /// permitindo aceleração progressiva, frenagem dianteira e traseira independente,
    /// puxada de grau (manete), troca sequencial de marchas e alternância de câmera.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/UI/Touchscreen Mobile Controls UI")]
    public class TouchscreenMobileControlsUI : MonoBehaviour
    {
        public static TouchscreenMobileControlsUI Instance { get; private set; }

        [Header("Referência de Entrada")]
        [SerializeField] private CrossPlatformInputHandler inputHandler;

        [Header("Visibilidade e Plataforma")]
        [Tooltip("Se verdadeiro, mantém os controles táteis visíveis mesmo no Editor Unity para testes de simulação.")]
        [SerializeField] private bool forceVisibleInEditor = true;

        [Header("Botões de Direção")]
        [SerializeField] private Button steerLeftButton;
        [SerializeField] private Button steerRightButton;

        [Header("Botões de Propulsão e Frenagem")]
        [SerializeField] private Button throttleButton;
        [SerializeField] private Button frontBrakeButton;
        [SerializeField] private Button rearBrakeButton;

        [Header("Manobras e Câmbio")]
        [SerializeField] private Button wheelieButton;
        [SerializeField] private Button shiftUpButton;
        [SerializeField] private Button shiftDownButton;
        [SerializeField] private Button cameraCycleButton;
        [SerializeField] private Button phoneToggleButton;
        [SerializeField] private Button resetButton;

        // Estados internos de toque contínuo
        private bool _isSteeringLeft = false;
        private bool _isSteeringRight = false;
        private bool _isThrottling = false;
        private bool _isFrontBraking = false;
        private bool _isRearBraking = false;
        private bool _isWheelieHolding = false;

        public bool IsThrottlePressed => _isThrottling;
        public bool IsWheeliePressed => _isWheelieHolding;
        public bool IsFrontBrakePressed => _isFrontBraking;
        public bool IsRearBrakePressed => _isRearBraking;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (inputHandler == null)
            {
                inputHandler = FindFirstObjectByType<CrossPlatformInputHandler>();
            }

            // Oculta os botões em PC standalone caso não esteja forçado para testes
            if (!Application.isMobilePlatform && !forceVisibleInEditor)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (inputHandler == null) return;

            // Transmite os estados contínuos para o handler
            if (_isSteeringLeft && !_isSteeringRight)
            {
                inputHandler.SetVirtualSteering(-1.0f);
            }
            else if (_isSteeringRight && !_isSteeringLeft)
            {
                inputHandler.SetVirtualSteering(1.0f);
            }
            else
            {
                inputHandler.SetVirtualSteering(0f);
            }

            inputHandler.SetVirtualThrottle(_isThrottling ? 1.0f : 0f);
            inputHandler.SetVirtualFrontBrake(_isFrontBraking ? 1.0f : 0f);
            inputHandler.SetVirtualRearBrake(_isRearBraking ? 1.0f : 0f);
            inputHandler.SetVirtualWheelie(_isWheelieHolding);
        }

        // --- Callbacks de Entrada Tátil (EventTrigger / OnPointerDown / OnPointerUp) ---

        public void OnThrottleDown() => _isThrottling = true;
        public void OnThrottleUp() => _isThrottling = false;

        public void OnSteerLeftDown() => _isSteeringLeft = true;
        public void OnSteerLeftUp() => _isSteeringLeft = false;

        public void OnSteerRightDown() => _isSteeringRight = true;
        public void OnSteerRightUp() => _isSteeringRight = false;

        public void OnFrontBrakeDown() => _isFrontBraking = true;
        public void OnFrontBrakeUp() => _isFrontBraking = false;

        public void OnRearBrakeDown() => _isRearBraking = true;
        public void OnRearBrakeUp() => _isRearBraking = false;

        public void OnWheelieDown() => _isWheelieHolding = true;
        public void OnWheelieUp() => _isWheelieHolding = false;

        public void OnShiftUpClicked()
        {
            if (inputHandler != null) inputHandler.TriggerVirtualShiftUp();
        }

        public void OnShiftDownClicked()
        {
            if (inputHandler != null) inputHandler.TriggerVirtualShiftDown();
        }

        public void OnCameraCycleClicked()
        {
            if (inputHandler != null) inputHandler.TriggerVirtualCameraCycle();
        }

        public void OnResetClicked()
        {
            if (inputHandler != null) inputHandler.TriggerVirtualReset();
        }

        public void OnPhoneToggleClicked()
        {
            var phone = FindFirstObjectByType<AthlonMobileApp>();
            if (phone != null)
            {
                phone.TogglePhone();
            }
        }
    }
}
