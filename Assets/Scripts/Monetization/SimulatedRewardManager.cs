using System;
using System.Collections;
using UnityEngine;
using ProjetoGrau2.Persistence;

namespace ProjetoGrau2.Monetization
{
    /// <summary>
    /// Gerenciador de recompensas simuladas 100% offline para RERY CITY.
    /// Fornece o fluxo de premiação sem depender de SDKs externos ou conexões com a internet.
    /// Possui proteção de clique duplo (debouncing), tempo de recarga (cooldown) e persistência.
    /// </summary>
    [AddComponentMenu("Rery City/Monetization/Simulated Reward Manager")]
    public class SimulatedRewardManager : MonoBehaviour
    {
        public static SimulatedRewardManager Instance { get; private set; }

        [Header("Configuração de Recompensa")]
        [SerializeField] private int baseRewardCoins = 500;
        [SerializeField] private float simulatedDurationSeconds = 2.0f;
        [SerializeField] private float cooldownSeconds = 15.0f;

        [Header("Estado")]
        [SerializeField] private bool isProcessing = false;
        [SerializeField] private float lastRewardTimestamp = -1000f;

        public event Action OnRewardProcessStarted;
        public event Action<int> OnRewardGranted;
        public event Action<string> OnRewardFailed;

        public bool IsProcessing => isProcessing;
        public float CooldownRemaining => Mathf.Max(0f, (lastRewardTimestamp + cooldownSeconds) - Time.time);
        public bool CanRequestReward => !isProcessing && CooldownRemaining <= 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Aciona a simulação de recompensa de forma segura e offline.
        /// </summary>
        public bool RequestSimulatedReward()
        {
            if (isProcessing)
            {
                OnRewardFailed?.Invoke("Recompensa já está sendo processada.");
                return false;
            }

            if (CooldownRemaining > 0f)
            {
                OnRewardFailed?.Invoke($"Aguarde {Mathf.CeilToInt(CooldownRemaining)}s para resgatar novamente.");
                return false;
            }

            StartCoroutine(ProcessSimulatedRewardRoutine());
            return true;
        }

        private IEnumerator ProcessSimulatedRewardRoutine()
        {
            isProcessing = true;
            OnRewardProcessStarted?.Invoke();

            if (simulatedDurationSeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(simulatedDurationSeconds);
            }

            lastRewardTimestamp = Time.time;
            isProcessing = false;

            ApplyReward(baseRewardCoins);
            OnRewardGranted?.Invoke(baseRewardCoins);
        }

        private void ApplyReward(int amount)
        {
            if (SaveSystem.Instance != null && SaveSystem.Instance.CurrentData != null)
            {
                SaveSystem.Instance.CurrentData.playerMoney += amount;
                SaveSystem.Instance.SaveGame();
            }
        }

        /// <summary>
        /// Reseta o cooldown para propósitos de teste ou inicialização limpa.
        /// </summary>
        public void ResetCooldownForTesting()
        {
            lastRewardTimestamp = -1000f;
            isProcessing = false;
        }
    }
}
