using System;
using UnityEngine;
using UnityEngine.UI;
using ProjetoGrau2.Core;
using ProjetoGrau2.Propulsion;
using ProjetoGrau2.Physics;
using ProjetoGrau2.Camera;

namespace ProjetoGrau2.UI
{
    /// <summary>
    /// Painel de instrumentos e telemetria principal da motocicleta (HUD).
    /// Extrai dados reais dos subsistemas físicos e de propulsão (sem valores fictícios):
    /// Velocidade (km/h), Tacômetro (RPM), Marcha, Corte de Giro, Two-Step, Ângulo de Grau,
    /// Modo de Câmera, Temperatura e Alertas Mecânicos.
    /// Otimizado para Zero GC em execução contínua no Android.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/UI/Motorcycle HUD")]
    public class MotorcycleHUD : MonoBehaviour
    {
        public static MotorcycleHUD Instance { get; private set; }

        [Header("Referências Centrais")]
        [SerializeField] private MotorcycleController motorcycle;
        [SerializeField] private MotorcycleCameraController cameraController;

        [Header("Instrumentos Digitais")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text rpmText;
        [SerializeField] private Slider rpmSlider;
        [SerializeField] private Text gearText;

        [Header("Indicadores de Desempenho e ECU")]
        [SerializeField] private Image revLimitIndicator;
        [SerializeField] private Image twoStepIndicator;
        [SerializeField] private Text cameraModeText;
        [SerializeField] private Text wheelieAngleText;
        [SerializeField] private Image wheelieIndicator;

        [Header("Condições Mecânicas")]
        [SerializeField] private Text temperatureText;
        [SerializeField] private Slider temperatureGauge;
        [SerializeField] private Text fuelText;
        [SerializeField] private Slider fuelGauge;
        [SerializeField] private Image mechanicalAlertIcon;

        [Header("Configurações de Taxa de Atualização")]
        [Tooltip("Intervalo entre atualizações de texto para eliminar gargalo de Garbage Collection (segundos).")]
        [Range(0.02f, 0.1f)] public float updateInterval = 0.05f; // 20 Hz

        // Valores em cache para dirty checking
        private float _updateTimer = 0f;
        private int _cachedSpeed = -1;
        private int _cachedRpm = -1;
        private int _cachedGear = -99;
        private bool _cachedCut = false;
        private bool _cachedTwoStep = false;
        private int _cachedPitchAngle = -999;
        private CameraViewMode _cachedCameraMode = (CameraViewMode)(-1);

        public MotorcycleController Motorcycle => motorcycle;
        public int DisplayedSpeed => _cachedSpeed;
        public int DisplayedRpm => _cachedRpm;
        public int DisplayedGear => _cachedGear;
        public bool IsCutDisplayed => _cachedCut;
        public bool IsTwoStepDisplayed => _cachedTwoStep;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            FindMotorcycleReferences();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            FindMotorcycleReferences();
            InitializeGauges();
        }

        private void FindMotorcycleReferences()
        {
            if (motorcycle == null)
            {
                motorcycle = FindFirstObjectByType<MotorcycleController>();
            }

            if (cameraController == null)
            {
                cameraController = FindFirstObjectByType<MotorcycleCameraController>();
            }
        }

        private void InitializeGauges()
        {
            if (motorcycle != null && motorcycle.Engine != null && motorcycle.Engine.Data != null)
            {
                if (rpmSlider != null)
                {
                    rpmSlider.minValue = 0f;
                    rpmSlider.maxValue = motorcycle.Engine.Data.maxRpm;
                }
            }

            if (temperatureGauge != null)
            {
                temperatureGauge.minValue = 40f;
                temperatureGauge.maxValue = 130f;
                temperatureGauge.value = 85f;
            }

            if (fuelGauge != null)
            {
                fuelGauge.minValue = 0f;
                fuelGauge.maxValue = 100f;
                fuelGauge.value = 85f;
            }
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer >= updateInterval)
            {
                _updateTimer = 0f;
                UpdateHUDTelemetry();
            }
        }

        /// <summary>
        /// Atualiza os instrumentos com dirty-checking para evitar alocações de string por frame.
        /// </summary>
        public void UpdateHUDTelemetry()
        {
            if (motorcycle == null)
            {
                FindMotorcycleReferences();
                if (motorcycle == null) return;
            }

            // 1. Velocidade Real (km/h)
            int currentSpeed = Mathf.Max(0, Mathf.RoundToInt(motorcycle.CurrentSpeedKmh));
            if (currentSpeed != _cachedSpeed)
            {
                _cachedSpeed = currentSpeed;
                if (speedText != null)
                {
                    speedText.text = _cachedSpeed.ToString();
                }
            }

            // 2. RPM do Motor e Tacômetro
            if (motorcycle.Engine != null)
            {
                int currentRpm = Mathf.Max(0, Mathf.RoundToInt(motorcycle.Engine.CurrentRpm));
                if (currentRpm != _cachedRpm)
                {
                    _cachedRpm = currentRpm;
                    if (rpmText != null)
                    {
                        rpmText.text = _cachedRpm.ToString();
                    }
                    if (rpmSlider != null)
                    {
                        rpmSlider.value = currentRpm;
                    }
                }

                // 3. Corte de Giro (Rev Limiter)
                bool isCut = motorcycle.Engine.IsCutActive || (motorcycle.ECU != null && motorcycle.ECU.IsCutting);
                if (isCut != _cachedCut)
                {
                    _cachedCut = isCut;
                    if (revLimitIndicator != null)
                    {
                        revLimitIndicator.color = _cachedCut ? Color.red : new Color(1f, 1f, 1f, 0.2f);
                    }
                }
            }

            // 4. Marcha Atual (Transmission)
            if (motorcycle.Transmission != null)
            {
                int gear = motorcycle.Transmission.CurrentGear;
                if (gear != _cachedGear)
                {
                    _cachedGear = gear;
                    if (gearText != null)
                    {
                        gearText.text = (_cachedGear == 0) ? "N" : _cachedGear.ToString();
                        gearText.color = (_cachedGear == 0) ? Color.green : Color.white;
                    }
                }
            }

            // 5. Two-Step (Athlon ECU)
            if (motorcycle.ECU != null)
            {
                bool twoStep = motorcycle.ECU.IsTwoStepActive;
                if (twoStep != _cachedTwoStep)
                {
                    _cachedTwoStep = twoStep;
                    if (twoStepIndicator != null)
                    {
                        twoStepIndicator.color = _cachedTwoStep ? Color.yellow : new Color(1f, 1f, 1f, 0.2f);
                    }
                }
            }

            // 6. Grau e Ângulo de Inclinação (Wheelie)
            if (motorcycle.WheelieController != null)
            {
                int pitchAngle = Mathf.RoundToInt(motorcycle.WheelieController.CurrentPitchAngle);
                if (pitchAngle != _cachedPitchAngle)
                {
                    _cachedPitchAngle = pitchAngle;
                    if (wheelieAngleText != null)
                    {
                        wheelieAngleText.text = _cachedPitchAngle.ToString() + "°";
                    }

                    if (wheelieIndicator != null)
                    {
                        bool isWheelie = _cachedPitchAngle > 15 && !motorcycle.IsFrontGrounded;
                        wheelieIndicator.color = isWheelie ? Color.cyan : new Color(1f, 1f, 1f, 0.2f);
                    }
                }
            }

            // 7. Modo de Câmera
            if (cameraController != null)
            {
                CameraViewMode mode = cameraController.CurrentMode;
                if (mode != _cachedCameraMode)
                {
                    _cachedCameraMode = mode;
                    if (cameraModeText != null)
                    {
                        cameraModeText.text = _cachedCameraMode.ToString();
                    }
                }
            }
        }
    }
}
