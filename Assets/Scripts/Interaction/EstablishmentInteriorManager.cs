using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Interaction
{
    [Serializable]
    public class InteriorEntry
    {
        public string interiorId;
        public GameObject interiorRoot;
        public Vector3 worldPosition;
        public float activationRadius = 20f;
        public float deactivationRadius = 28f; // Histerese para evitar flicker
        public bool isStreamingActive = false;
    }

    /// <summary>
    /// Gerenciador de streaming leve para interiores de estabelecimentos urbanos (oficinas, lojas, postos).
    /// Alterna a ativação de objetos internos com base em proximidade e histerese sem alocação de GC,
    /// garantindo 60 FPS contínuos em dispositivos Android sem telas de carregamento.
    /// </summary>
    [AddComponentMenu("Rery City/Interaction/Establishment Interior Manager")]
    public class EstablishmentInteriorManager : MonoBehaviour
    {
        public static EstablishmentInteriorManager Instance { get; private set; }

        [Header("Alvo de Rastreamento")]
        [SerializeField] private Transform playerTransform;

        [Header("Configurações de Streaming")]
        [SerializeField] private float checkInterval = 0.25f;
        [SerializeField] private List<InteriorEntry> registeredInteriors = new List<InteriorEntry>();

        private float _checkTimer = 0f;
        private int _activeInteriorCount = 0;

        public int ActiveInteriorCount => _activeInteriorCount;
        public int RegisteredCount => registeredInteriors.Count;
        public IReadOnlyList<InteriorEntry> RegisteredInteriors => registeredInteriors;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (playerTransform == null && Camera.main != null)
            {
                playerTransform = Camera.main.transform;
            }

            // Inicializa estado dos interiores registrados
            for (int i = 0; i < registeredInteriors.Count; i++)
            {
                var entry = registeredInteriors[i];
                if (entry.interiorRoot != null)
                {
                    entry.interiorRoot.SetActive(entry.isStreamingActive);
                }
            }
        }

        private void Update()
        {
            if (registeredInteriors.Count == 0) return;

            _checkTimer += Time.deltaTime;
            if (_checkTimer >= checkInterval)
            {
                _checkTimer = 0f;
                Vector3 targetPos = playerTransform != null ? playerTransform.position : Vector3.zero;
                EvaluateInteriors(targetPos);
            }
        }

        public void RegisterInterior(string interiorId, GameObject interiorRoot, Vector3 worldPosition, float activationRadius = 20f, float deactivationRadius = 28f)
        {
            if (interiorRoot == null) return;

            for (int i = 0; i < registeredInteriors.Count; i++)
            {
                if (registeredInteriors[i].interiorId == interiorId)
                {
                    registeredInteriors[i].interiorRoot = interiorRoot;
                    registeredInteriors[i].worldPosition = worldPosition;
                    return;
                }
            }

            var entry = new InteriorEntry
            {
                interiorId = interiorId,
                interiorRoot = interiorRoot,
                worldPosition = worldPosition,
                activationRadius = activationRadius,
                deactivationRadius = Mathf.Max(activationRadius + 2f, deactivationRadius),
                isStreamingActive = false
            };

            interiorRoot.SetActive(false);
            registeredInteriors.Add(entry);
        }

        public void EvaluateInteriors(Vector3 observerPosition)
        {
            _activeInteriorCount = 0;
            for (int i = 0; i < registeredInteriors.Count; i++)
            {
                var entry = registeredInteriors[i];
                if (entry.interiorRoot == null) continue;

                float distSqr = (entry.worldPosition - observerPosition).sqrMagnitude;
                float actDistSqr = entry.activationRadius * entry.activationRadius;
                float deactDistSqr = entry.deactivationRadius * entry.deactivationRadius;

                if (!entry.isStreamingActive && distSqr <= actDistSqr)
                {
                    entry.isStreamingActive = true;
                    entry.interiorRoot.SetActive(true);
                }
                else if (entry.isStreamingActive && distSqr > deactDistSqr)
                {
                    entry.isStreamingActive = false;
                    entry.interiorRoot.SetActive(false);
                }

                if (entry.isStreamingActive)
                {
                    _activeInteriorCount++;
                }
            }
        }

        public void ForceInteriorState(string interiorId, bool active)
        {
            for (int i = 0; i < registeredInteriors.Count; i++)
            {
                var entry = registeredInteriors[i];
                if (entry.interiorId == interiorId)
                {
                    entry.isStreamingActive = active;
                    if (entry.interiorRoot != null)
                    {
                        entry.interiorRoot.SetActive(active);
                    }
                    break;
                }
            }
        }

        public void SetPlayerTransformForTesting(Transform target)
        {
            playerTransform = target;
        }
    }
}
