using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjetoGrau2.Interaction
{
    /// <summary>
    /// Gerenciador de interface contextual para interação com estabelecimentos e elementos urbanos.
    /// Exibe botão tátil e prompt de texto dinamicamente quando um interactable está ao alcance.
    /// </summary>
    [AddComponentMenu("Rery City/Interaction/Interaction Context UI")]
    public class InteractionContextUI : MonoBehaviour
    {
        public static InteractionContextUI Instance { get; private set; }

        [Header("Elementos de UI")]
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private Text promptText;
        [SerializeField] private Button interactButton;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Configurações")]
        [SerializeField] private float fadeSpeed = 8f;
        [SerializeField] private string defaultActionPrompt = "Interagir";

        private PlayerInteractionDetector _detector;
        private bool _isPromptVisible = false;
        private float _targetAlpha = 0f;

        public bool IsPromptVisible => _isPromptVisible;
        public string CurrentPromptText => promptText != null ? promptText.text : string.Empty;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }
        }

        private void Start()
        {
            if (_detector == null)
            {
                _detector = PlayerInteractionDetector.Instance;
            }
            BindDetector(_detector);
            SetPromptVisibility(false, true);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
            UnbindDetector(_detector);
        }

        private void Update()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, _targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
                if (interactionPanel != null && canvasGroup.alpha <= 0.001f && !_isPromptVisible)
                {
                    interactionPanel.SetActive(false);
                }
            }
        }

        public void BindDetector(PlayerInteractionDetector detector)
        {
            if (detector == null) return;
            _detector = detector;
            _detector.OnInteractableDetected += HandleInteractableDetected;
            _detector.OnInteractableLost += HandleInteractableLost;
            _detector.OnInteractionExecuted += HandleInteractionExecuted;
            _detector.OnInteractionEnded += HandleInteractionEnded;

            if (_detector.HasInteractable)
            {
                HandleInteractableDetected(_detector.CurrentInteractable);
            }
        }

        public void UnbindDetector(PlayerInteractionDetector detector)
        {
            if (detector == null) return;
            detector.OnInteractableDetected -= HandleInteractableDetected;
            detector.OnInteractableLost -= HandleInteractableLost;
            detector.OnInteractionExecuted -= HandleInteractionExecuted;
            detector.OnInteractionEnded -= HandleInteractionEnded;
        }

        private void HandleInteractableDetected(IInteractable interactable)
        {
            if (interactable == null) return;
            string text = interactable.InteractionPrompt;
            if (string.IsNullOrEmpty(text))
            {
                text = defaultActionPrompt;
            }
            ShowPrompt(text);
        }

        private void HandleInteractableLost()
        {
            HidePrompt();
        }

        private void HandleInteractionExecuted(IInteractable interactable)
        {
            // Oculta prompt de aproximação enquanto estiver no menu ou atividade interna
            HidePrompt();
        }

        private void HandleInteractionEnded()
        {
            // Se ainda houver interactable próximo após fechar menu, reexibe
            if (_detector != null && _detector.HasInteractable)
            {
                HandleInteractableDetected(_detector.CurrentInteractable);
            }
        }

        public void ShowPrompt(string message)
        {
            if (promptText != null)
            {
                promptText.text = message;
            }
            SetPromptVisibility(true, false);
        }

        public void HidePrompt()
        {
            SetPromptVisibility(false, false);
        }

        private void SetPromptVisibility(bool visible, bool immediate)
        {
            _isPromptVisible = visible;
            _targetAlpha = visible ? 1f : 0f;

            if (interactionPanel != null && visible)
            {
                interactionPanel.SetActive(true);
            }

            if (canvasGroup != null && immediate)
            {
                canvasGroup.alpha = _targetAlpha;
                if (!visible && interactionPanel != null)
                {
                    interactionPanel.SetActive(false);
                }
            }
        }

        public void OnInteractButtonClicked()
        {
            if (_detector != null)
            {
                _detector.TriggerInteraction();
            }
        }

        public void SetupForTesting(PlayerInteractionDetector detector, Text textComponent, Button buttonComponent, CanvasGroup group, GameObject panel)
        {
            _detector = detector;
            promptText = textComponent;
            interactButton = buttonComponent;
            canvasGroup = group;
            interactionPanel = panel;
            BindDetector(_detector);
        }
    }
}
