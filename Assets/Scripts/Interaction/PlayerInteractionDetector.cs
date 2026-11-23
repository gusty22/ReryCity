using System;
using UnityEngine;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.Interaction
{
    /// <summary>
    /// Componente acoplado ao jogador/motocicleta para detecção de entidades interativas próximas.
    /// Gerencia bloqueio de comandos veiculares durante interações e emite eventos para a UI.
    /// </summary>
    [AddComponentMenu("Rery City/Interaction/Player Interaction Detector")]
    public class PlayerInteractionDetector : MonoBehaviour
    {
        public static PlayerInteractionDetector Instance { get; private set; }

        [Header("Configurações de Proximidade")]
        [SerializeField] private float detectionRadius = 3.5f;
        [SerializeField] private LayerMask interactableLayer = ~0;
        [SerializeField] private float checkInterval = 0.2f;

        [Header("Estado")]
        [SerializeField] private bool isInteracting = false;
        private IInteractable _currentInteractable;
        private float _checkTimer = 0f;

        // Buffers não-alocativos para Zero GC
        private readonly Collider[] _hitBuffer = new Collider[8];

        public event Action<IInteractable> OnInteractableDetected;
        public event Action OnInteractableLost;
        public event Action<IInteractable> OnInteractionExecuted;
        public event Action OnInteractionEnded;

        public IInteractable CurrentInteractable => _currentInteractable;
        public bool HasInteractable => _currentInteractable != null;
        public bool IsInteracting => isInteracting;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (isInteracting) return;

            _checkTimer += Time.deltaTime;
            if (_checkTimer >= checkInterval)
            {
                _checkTimer = 0f;
                DetectNearbyInteractable();
            }

            // Suporte a teclado no PC
            if (_currentInteractable != null && Input.GetKeyDown(KeyCode.E))
            {
                TriggerInteraction();
            }
        }

        public void DetectNearbyInteractable()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _hitBuffer, interactableLayer);
            IInteractable bestCandidate = null;
            float bestDistanceSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var col = _hitBuffer[i];
                if (col == null || col.gameObject == gameObject) continue;

                var candidate = col.GetComponent<IInteractable>() ?? col.GetComponentInParent<IInteractable>();
                if (candidate != null && candidate.CanInteract(gameObject))
                {
                    float dSqr = (col.transform.position - transform.position).sqrMagnitude;
                    if (dSqr < bestDistanceSqr && dSqr <= (candidate.MaxInteractionDistance * candidate.MaxInteractionDistance))
                    {
                        bestDistanceSqr = dSqr;
                        bestCandidate = candidate;
                    }
                }
            }

            if (bestCandidate != _currentInteractable)
            {
                _currentInteractable = bestCandidate;
                if (_currentInteractable != null)
                {
                    OnInteractableDetected?.Invoke(_currentInteractable);
                }
                else
                {
                    OnInteractableLost?.Invoke();
                }
            }
        }

        /// <summary>
        /// Acionado por botão tátil na UI mobile ou tecla de ação no PC.
        /// </summary>
        public bool TriggerInteraction()
        {
            if (_currentInteractable == null || isInteracting) return false;

            if (_currentInteractable.CanInteract(gameObject))
            {
                isInteracting = true;
                _currentInteractable.Interact(gameObject);
                OnInteractionExecuted?.Invoke(_currentInteractable);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finaliza o modo de interação, reabilitando os controles normais da motocicleta.
        /// </summary>
        public void CompleteInteraction()
        {
            if (!isInteracting) return;

            if (_currentInteractable != null)
            {
                _currentInteractable.CancelInteraction(gameObject);
            }

            isInteracting = false;
            OnInteractionEnded?.Invoke();
            DetectNearbyInteractable();
        }

        public void SetCurrentInteractableForTesting(IInteractable interactable)
        {
            _currentInteractable = interactable;
        }
    }
}
