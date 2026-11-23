using System;
using UnityEngine;
using ProjetoGrau2.Persistence;

namespace ProjetoGrau2.Interaction
{
    /// <summary>
    /// Representa um estabelecimento comercial ou de serviço interativo em RERY CITY (oficinas, borracharias, mercadinhos).
    /// Suporta estacionamento de motocicletas, solicitação de serviços debitados da economia persistente e streaming de interiores.
    /// </summary>
    [AddComponentMenu("Rery City/Interaction/Interactive Establishment")]
    public class InteractiveEstablishment : MonoBehaviour, IInteractable
    {
        [Header("Configuração")]
        [SerializeField] private InteractiveEstablishmentConfig config;

        [Header("Pontos de Referência")]
        [SerializeField] private Transform parkingSpot;
        [SerializeField] private Transform serviceBay;
        [SerializeField] private Transform customerCounter;

        [Header("Estrutura Interna")]
        [SerializeField] private GameObject interiorRoot;
        [SerializeField] private float interiorActivationRadius = 18f;

        [Header("Estado")]
        [SerializeField] private bool isPlayerInside = false;

        public event Action<InteractiveEstablishment> OnPlayerEntered;
        public event Action<InteractiveEstablishment> OnPlayerExited;
        public event Action<string, int> OnServiceExecuted;

        // Implementação IInteractable
        public string InteractionId => config != null ? config.establishmentId : name;

        public string PromptText
        {
            get
            {
                string estName = config != null ? config.establishmentName : name;
                return isPlayerInside ? $"Sair de {estName}" : $"Entrar em {estName}";
            }
        }

        public string InteractionPrompt => PromptText;

        public InteractionType Type
        {
            get
            {
                if (config == null) return InteractionType.EntrarEstabelecimento;
                return config.category switch
                {
                    EstablishmentCategory.Workshop => InteractionType.ManutencaoMoto,
                    EstablishmentCategory.GasStation => InteractionType.AbastecerCombustivel,
                    EstablishmentCategory.TireShop => InteractionType.ServicoBorracharia,
                    EstablishmentCategory.AutoParts => InteractionType.ComprarPeca,
                    _ => InteractionType.EntrarEstabelecimento
                };
            }
        }

        public float MaxInteractionDistance => config != null ? config.interactionRadius : 4.0f;
        public bool IsPlayerInside => isPlayerInside;
        public InteractiveEstablishmentConfig Config => config;
        public Transform ParkingSpot => parkingSpot;
        public Transform ServiceBay => serviceBay;

        private void Start()
        {
            if (interiorRoot != null)
            {
                if (EstablishmentInteriorManager.Instance != null)
                {
                    EstablishmentInteriorManager.Instance.RegisterInterior(
                        InteractionId,
                        interiorRoot,
                        transform.position,
                        interiorActivationRadius,
                        interiorActivationRadius + 8f
                    );
                }
                else
                {
                    interiorRoot.SetActive(false);
                }
            }
        }

        public bool CanInteract(GameObject interactor)
        {
            if (interactor == null) return false;
            float distSqr = (interactor.transform.position - transform.position).sqrMagnitude;
            if (distSqr > (MaxInteractionDistance * MaxInteractionDistance)) return false;

            if (config != null && !config.isOpen24Hours)
            {
                // Em um ciclo de dia/noite real poderíamos ler o relógio do jogo; assume 12h se não configurado
                return config.IsCurrentlyOpen(12.0f);
            }

            return true;
        }

        public void Interact(GameObject interactor)
        {
            isPlayerInside = !isPlayerInside;

            if (interiorRoot != null)
            {
                interiorRoot.SetActive(isPlayerInside);
            }

            if (isPlayerInside)
            {
                OnPlayerEntered?.Invoke(this);
            }
            else
            {
                OnPlayerExited?.Invoke(this);
            }
        }

        public void CancelInteraction(GameObject interactor)
        {
            if (isPlayerInside)
            {
                isPlayerInside = false;
                if (interiorRoot != null)
                {
                    interiorRoot.SetActive(false);
                }
                OnPlayerExited?.Invoke(this);
            }
        }

        /// <summary>
        /// Estaciona a motocicleta na vaga designada do estabelecimento.
        /// </summary>
        public bool ParkVehicle(GameObject vehicle)
        {
            if (vehicle == null || parkingSpot == null) return false;

            vehicle.transform.position = parkingSpot.position;
            vehicle.transform.rotation = parkingSpot.rotation;

            var rb = vehicle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            return true;
        }

        /// <summary>
        /// Executa um serviço comercial debitando da carteira persistente do jogador via SaveSystem.
        /// </summary>
        public bool RequestService(string serviceName, int costCentavos)
        {
            var data = SaveSystem.Load();
            if (data == null)
            {
                data = new PlayerProgressData();
            }

            if (data.playerMoneyCentavos < costCentavos)
            {
                Debug.LogWarning($"[InteractiveEstablishment] Saldo insuficiente para {serviceName}. Necessário: R$ {costCentavos / 100f:F2}, Disponível: R$ {data.playerMoneyCentavos / 100f:F2}");
                return false;
            }

            data.playerMoneyCentavos -= costCentavos;
            SaveSystem.Save(data);

            OnServiceExecuted?.Invoke(serviceName, costCentavos);
            return true;
        }

        public void SetupForTesting(InteractiveEstablishmentConfig testConfig, GameObject testInterior, Transform testParking)
        {
            config = testConfig;
            interiorRoot = testInterior;
            parkingSpot = testParking;
        }
    }
}
