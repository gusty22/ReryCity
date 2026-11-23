using System;
using UnityEngine;
using UnityEngine.UI;
using ProjetoGrau2.Police;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.UI
{
    /// <summary>
    /// Painel de exibição e telemetria de perseguição policial (HUD).
    /// Mostra estado tático da perseguição, distância da viatura mais próxima,
    /// popups de infrações cometidas e progresso de abordagem/prisão.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/UI/Police HUD")]
    public class PoliceHUD : MonoBehaviour
    {
        public static PoliceHUD Instance { get; private set; }

        [Header("Elementos de Texto")]
        [SerializeField] private Text statusBannerText;
        [SerializeField] private Text nearestPoliceDistanceText;
        [SerializeField] private Text violationAlertText;

        [Header("Elementos de Progresso")]
        [SerializeField] private Slider arrestProgressBar;
        [SerializeField] private Slider evasionProgressBar;

        [Header("Efeitos de Sirene")]
        [SerializeField] private Image sirenFlashOverlay;

        [Header("Referências")]
        [SerializeField] private MotorcycleController playerMotorcycle;

        private float _alertTimer = 0f;
        private PolicePursuitController[] _activePoliceUnits;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (playerMotorcycle == null)
            {
                playerMotorcycle = FindFirstObjectByType<MotorcycleController>();
            }
        }

        private void Start()
        {
            if (TrafficViolationDetector.Instance != null)
            {
                TrafficViolationDetector.Instance.OnViolationCommitted += HandleViolationCommitted;
            }
            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.OnEvasionProgressUpdated += HandleEvasionProgress;
            }

            _activePoliceUnits = FindObjectsByType<PolicePursuitController>(FindObjectsSortMode.None);
        }

        private void OnDestroy()
        {
            if (TrafficViolationDetector.Instance != null)
            {
                TrafficViolationDetector.Instance.OnViolationCommitted -= HandleViolationCommitted;
            }
            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.OnEvasionProgressUpdated -= HandleEvasionProgress;
            }
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            UpdateViolationAlertTimer(Time.deltaTime);
            UpdateNearestPoliceDistance();
            UpdateArrestProgress();
            UpdateSirenFlash();
        }

        private void HandleViolationCommitted(TrafficViolationType type, int score, string desc)
        {
            if (violationAlertText != null)
            {
                violationAlertText.text = desc;
                violationAlertText.gameObject.SetActive(true);
                _alertTimer = 3.5f;
            }
        }

        private void HandleEvasionProgress(float progress)
        {
            if (evasionProgressBar != null)
            {
                bool show = progress > 0f && progress < 1f;
                evasionProgressBar.gameObject.SetActive(show);
                evasionProgressBar.value = progress;
            }
        }

        private void UpdateViolationAlertTimer(float dt)
        {
            if (_alertTimer > 0f)
            {
                _alertTimer -= dt;
                if (_alertTimer <= 0f && violationAlertText != null)
                {
                    violationAlertText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateNearestPoliceDistance()
        {
            if (playerMotorcycle == null || _activePoliceUnits == null || _activePoliceUnits.Length == 0)
            {
                if (nearestPoliceDistanceText != null) nearestPoliceDistanceText.text = "";
                return;
            }

            float minDistance = float.MaxValue;
            PolicePursuitController closestPolice = null;

            for (int i = 0; i < _activePoliceUnits.Length; i++)
            {
                var unit = _activePoliceUnits[i];
                if (unit == null || !unit.gameObject.activeInHierarchy) continue;

                float d = Vector3.Distance(playerMotorcycle.transform.position, unit.transform.position);
                if (d < minDistance)
                {
                    minDistance = d;
                    closestPolice = unit;
                }
            }

            if (closestPolice != null)
            {
                if (nearestPoliceDistanceText != null)
                {
                    nearestPoliceDistanceText.text = $"Viatura mais próxima: {minDistance:F0}m";
                }

                if (statusBannerText != null)
                {
                    statusBannerText.text = closestPolice.CurrentState switch
                    {
                        PoliceAIState.Patrol => "PATRULHAMENTO DE ROTINA",
                        PoliceAIState.Suspicious => "SUSPEITA DETECTADA",
                        PoliceAIState.Investigating => "AVERIGUAÇÃO POLICIAL",
                        PoliceAIState.Pursuit => "EM PERSEGUIÇÃO!",
                        PoliceAIState.Intercepting => "TENTATIVA DE INTERCEPTAÇÃO!",
                        PoliceAIState.Arresting => "ORDEM DE PARADA / ABORDAGEM!",
                        PoliceAIState.LostTarget => "BUSCA POR FUGITIVO...",
                        _ => ""
                    };
                }
            }
        }

        private void UpdateArrestProgress()
        {
            if (arrestProgressBar == null || _activePoliceUnits == null) return;

            float maxArrest = 0f;
            for (int i = 0; i < _activePoliceUnits.Length; i++)
            {
                var unit = _activePoliceUnits[i];
                if (unit != null && unit.CurrentState == PoliceAIState.Arresting)
                {
                    maxArrest = Mathf.Max(maxArrest, unit.ArrestProgress);
                }
            }

            arrestProgressBar.gameObject.SetActive(maxArrest > 0f);
            arrestProgressBar.value = maxArrest;
        }

        private void UpdateSirenFlash()
        {
            if (sirenFlashOverlay == null) return;

            int level = (WantedLevelManager.Instance != null) ? WantedLevelManager.Instance.CurrentWantedLevel : 0;
            if (level >= 3)
            {
                sirenFlashOverlay.gameObject.SetActive(true);
                float flash = Mathf.PingPong(Time.time * 3.5f, 0.25f);
                sirenFlashOverlay.color = new Color(1f, 0f, 0f, flash);
            }
            else
            {
                sirenFlashOverlay.gameObject.SetActive(false);
            }
        }
    }
}
