using UnityEngine;

namespace ProjetoGrau2.Inputs
{
    /// <summary>
    /// Gerenciador de entrada multiplataforma para PC (Teclado/Mouse/Gamepad) e Mobile (Touch UI).
    /// Implementa o contrato IMotorcycleInput completo para física, câmbio e câmeras.
    /// </summary>
    public class CrossPlatformInputHandler : MonoBehaviour, IMotorcycleInput
    {
        [Header("Teclas de Locomoção (PC)")]
        [SerializeField] private KeyCode throttleKey = KeyCode.W;
        [SerializeField] private KeyCode reverseKey = KeyCode.S;
        [SerializeField] private KeyCode steerLeftKey = KeyCode.A;
        [SerializeField] private KeyCode steerRightKey = KeyCode.D;

        [Header("Teclas de Manobras e Freios (PC)")]
        [SerializeField] private KeyCode wheelieKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode rearBrakeKey = KeyCode.Space;
        [SerializeField] private KeyCode frontBrakeKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode clutchKey = KeyCode.C;
        [SerializeField] private KeyCode resetKey = KeyCode.R;

        [Header("Teclas de Câmbio (PC)")]
        [SerializeField] private KeyCode shiftUpKey = KeyCode.E;
        [SerializeField] private KeyCode shiftDownKey = KeyCode.Q;

        [Header("Controle de Câmeras (PC)")]
        [SerializeField] private KeyCode cameraCycleKey = KeyCode.V;
        [SerializeField] private float mouseSensitivity = 2.0f;

        // Entradas virtuais de Mobile UI
        private float _virtualThrottle;
        private float _virtualSteering;
        private bool _virtualWheelie;
        private float _virtualRearBrake;
        private float _virtualFrontBrake;
        private float _virtualClutch;
        private bool _virtualReset;
        private bool _virtualShiftUp;
        private bool _virtualShiftDown;
        private bool _virtualCameraCycle;
        private Vector2 _virtualLookDelta;

        public float Throttle { get; private set; }
        public float FrontBrake { get; private set; }
        public float RearBrake { get; private set; }
        public float Steering { get; private set; }
        public bool WheelieHold { get; private set; }
        public bool ResetTriggered { get; private set; }
        public bool ShiftUpTriggered { get; private set; }
        public bool ShiftDownTriggered { get; private set; }
        public float Clutch { get; private set; }
        public bool CameraCycleTriggered { get; private set; }
        public Vector2 LookDelta { get; private set; }

        private void Update()
        {
            // Processamento PC
            float pcThrottle = 0f;
            if (UnityEngine.Input.GetKey(throttleKey)) pcThrottle += 1f;
            if (UnityEngine.Input.GetKey(reverseKey)) pcThrottle -= 0.5f;

            float pcSteer = 0f;
            if (UnityEngine.Input.GetKey(steerRightKey)) pcSteer += 1f;
            if (UnityEngine.Input.GetKey(steerLeftKey)) pcSteer -= 1f;

            bool pcWheelie = UnityEngine.Input.GetKey(wheelieKey);
            float pcRearBrake = UnityEngine.Input.GetKey(rearBrakeKey) ? 1f : 0f;
            float pcFrontBrake = UnityEngine.Input.GetKey(frontBrakeKey) ? 1f : 0f;
            float pcClutch = UnityEngine.Input.GetKey(clutchKey) ? 1f : 0f;
            bool pcReset = UnityEngine.Input.GetKeyDown(resetKey);
            bool pcShiftUp = UnityEngine.Input.GetKeyDown(shiftUpKey);
            bool pcShiftDown = UnityEngine.Input.GetKeyDown(shiftDownKey);
            bool pcCameraCycle = UnityEngine.Input.GetKeyDown(cameraCycleKey);

            Vector2 pcLook = Vector2.zero;
            if (UnityEngine.Input.GetMouseButton(1) || Mathf.Abs(UnityEngine.Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(UnityEngine.Input.GetAxis("Mouse Y")) > 0.01f)
            {
                pcLook = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")) * mouseSensitivity;
            }

            // Combinação PC + Mobile Virtual
            Throttle = Mathf.Clamp(pcThrottle + _virtualThrottle, -1f, 1f);
            Steering = Mathf.Clamp(pcSteer + _virtualSteering, -1f, 1f);
            WheelieHold = pcWheelie || _virtualWheelie;
            RearBrake = Mathf.Clamp01(pcRearBrake + _virtualRearBrake);
            FrontBrake = Mathf.Clamp01(pcFrontBrake + _virtualFrontBrake);
            Clutch = Mathf.Clamp01(pcClutch + _virtualClutch);
            ResetTriggered = pcReset || _virtualReset;
            ShiftUpTriggered = pcShiftUp || _virtualShiftUp;
            ShiftDownTriggered = pcShiftDown || _virtualShiftDown;
            CameraCycleTriggered = pcCameraCycle || _virtualCameraCycle;
            LookDelta = pcLook + _virtualLookDelta;

            // Consumo de pulsos virtuais
            if (_virtualReset) _virtualReset = false;
            if (_virtualShiftUp) _virtualShiftUp = false;
            if (_virtualShiftDown) _virtualShiftDown = false;
            if (_virtualCameraCycle) _virtualCameraCycle = false;
            _virtualLookDelta = Vector2.zero;
        }

        #region Métodos de Interface para Mobile UI
        public void SetVirtualThrottle(float value) => _virtualThrottle = Mathf.Clamp01(value);
        public void SetVirtualSteering(float value) => _virtualSteering = Mathf.Clamp(value, -1f, 1f);
        public void SetVirtualWheelie(bool isPressed) => _virtualWheelie = isPressed;
        public void SetVirtualRearBrake(float value) => _virtualRearBrake = Mathf.Clamp01(value);
        public void SetVirtualFrontBrake(float value) => _virtualFrontBrake = Mathf.Clamp01(value);
        public void SetVirtualClutch(float value) => _virtualClutch = Mathf.Clamp01(value);
        public void TriggerVirtualReset() => _virtualReset = true;
        public void TriggerVirtualShiftUp() => _virtualShiftUp = true;
        public void TriggerVirtualShiftDown() => _virtualShiftDown = true;
        public void TriggerVirtualCameraCycle() => _virtualCameraCycle = true;
        public void SetVirtualLookDelta(Vector2 delta) => _virtualLookDelta = delta;
        #endregion
    }
}
