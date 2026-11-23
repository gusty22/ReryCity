using System;
using UnityEngine;
using ProjetoGrau2.Persistence;

namespace ProjetoGrau2.Interaction
{
    /// <summary>
    /// Bomba de abastecimento interativa para postos de combustíveis em RERY CITY ("Posto Pioneiro").
    /// Permite abastecer a motocicleta com cálculo dinâmico de litros x preço e débito no SaveSystem.
    /// </summary>
    [AddComponentMenu("Rery City/Interaction/Gas Station Pump")]
    public class GasStationPump : MonoBehaviour, IInteractable
    {
        [Header("Configuração da Bomba")]
        [SerializeField] private string pumpId = "pump_araguari_01";
        [SerializeField] private string fuelType = "Gasolina Comum";
        [SerializeField] private int pricePerLiterCentavos = 589; // R$ 5,89/L
        [SerializeField] private float maxInteractionDistance = 3.5f;

        [Header("Especificações de Abastecimento")]
        [SerializeField] private float tankCapacityLiters = 14.0f; // Capacidade padrão 150/160cc
        [SerializeField] private float currentFuelLiters = 3.5f;
        [SerializeField] private float flowRateLitersPerSecond = 2.0f;

        [Header("Estado")]
        [SerializeField] private bool isFueling = false;

        public event Action<float, int> OnRefuelCompleted; // litros, custo em centavos
        public event Action OnRefuelStarted;
        public event Action OnRefuelStopped;

        // Implementação IInteractable
        public string InteractionId => pumpId;
        public string PromptText => $"Abastecer {fuelType} (R$ {pricePerLiterCentavos / 100f:F2}/L)";
        public string InteractionPrompt => PromptText;
        public InteractionType Type => InteractionType.AbastecerCombustivel;
        public float MaxInteractionDistance => maxInteractionDistance;

        public string FuelType => fuelType;
        public int PricePerLiterCentavos => pricePerLiterCentavos;
        public float CurrentFuelLiters => currentFuelLiters;
        public float TankCapacityLiters => tankCapacityLiters;
        public float FuelPercentage => tankCapacityLiters > 0f ? (currentFuelLiters / tankCapacityLiters) : 0f;
        public bool IsFueling => isFueling;

        public bool CanInteract(GameObject interactor)
        {
            if (interactor == null) return false;
            float distSqr = (interactor.transform.position - transform.position).sqrMagnitude;
            return distSqr <= (maxInteractionDistance * maxInteractionDistance);
        }

        public void Interact(GameObject interactor)
        {
            if (isFueling)
            {
                CancelInteraction(interactor);
                return;
            }

            // Realiza abastecimento completo ou até o limite do saldo
            RefuelFullTank();
        }

        public void CancelInteraction(GameObject interactor)
        {
            if (isFueling)
            {
                isFueling = false;
                OnRefuelStopped?.Invoke();
            }
        }

        /// <summary>
        /// Abastece uma quantidade específica de litros, debitando o valor correspondente do SaveSystem.
        /// </summary>
        public bool RefuelLiters(float litersToFill)
        {
            if (litersToFill <= 0f) return false;

            float spaceAvailable = Mathf.Max(0f, tankCapacityLiters - currentFuelLiters);
            float actualLiters = Mathf.Min(litersToFill, spaceAvailable);

            if (actualLiters <= 0.001f)
            {
                Debug.Log("[GasStationPump] Tanque já está cheio.");
                return false;
            }

            int costCentavos = Mathf.CeilToInt(actualLiters * pricePerLiterCentavos);

            var data = SaveSystem.Load() ?? new PlayerProgressData();

            if (data.playerMoneyCentavos < costCentavos)
            {
                // Se não tem dinheiro suficiente para tudo, abastece o que o saldo puder pagar
                if (data.playerMoneyCentavos < pricePerLiterCentavos)
                {
                    Debug.LogWarning("[GasStationPump] Saldo insuficiente para abastecer ao menos 1 litro.");
                    return false;
                }

                actualLiters = (float)data.playerMoneyCentavos / pricePerLiterCentavos;
                actualLiters = Mathf.Min(actualLiters, spaceAvailable);
                costCentavos = Mathf.FloorToInt(actualLiters * pricePerLiterCentavos);
            }

            data.playerMoneyCentavos -= costCentavos;
            SaveSystem.Save(data);

            currentFuelLiters += actualLiters;
            currentFuelLiters = Mathf.Min(currentFuelLiters, tankCapacityLiters);

            OnRefuelCompleted?.Invoke(actualLiters, costCentavos);
            return true;
        }

        /// <summary>
        /// Enche o tanque completamente até a capacidade máxima.
        /// </summary>
        public bool RefuelFullTank()
        {
            float needed = tankCapacityLiters - currentFuelLiters;
            if (needed <= 0.01f) return false;

            OnRefuelStarted?.Invoke();
            isFueling = true;
            bool success = RefuelLiters(needed);
            isFueling = false;
            OnRefuelStopped?.Invoke();

            return success;
        }

        /// <summary>
        /// Abastece um valor monetário fixo (ex: R$ 20,00 ou R$ 50,00).
        /// </summary>
        public bool RefuelByValueCentavos(int valueCentavos)
        {
            if (valueCentavos <= 0 || pricePerLiterCentavos <= 0) return false;
            float liters = (float)valueCentavos / pricePerLiterCentavos;
            return RefuelLiters(liters);
        }

        public void SetupForTesting(string id, int priceCentavos, float capacity, float currentFuel)
        {
            pumpId = id;
            pricePerLiterCentavos = priceCentavos;
            tankCapacityLiters = capacity;
            currentFuelLiters = currentFuel;
        }
    }
}
