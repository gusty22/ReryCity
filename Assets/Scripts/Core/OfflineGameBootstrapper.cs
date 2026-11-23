using System;
using UnityEngine;
using ProjetoGrau2.Persistence;

namespace ProjetoGrau2.Core
{
    /// <summary>
    /// Gerencia a inicialização desacoplada e 100% offline de RERY CITY.
    /// Assegura carregamento de subsistemas vitais locais sem qualquer requisição de rede ou anúncios externos.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Rery City/Core/Offline Game Bootstrapper")]
    public class OfflineGameBootstrapper : MonoBehaviour
    {
        public static OfflineGameBootstrapper Instance { get; private set; }

        public enum BootstrapState
        {
            Uninitialized,
            VerifyingLocalEnvironment,
            LoadingLocalData,
            InitializingCoreSubsystems,
            Ready
        }

        [Header("Configuração de Identidade")]
        [SerializeField] private ReryCityIdentityConfig identityConfig;

        [Header("Estado de Inicialização")]
        [SerializeField] private BootstrapState currentState = BootstrapState.Uninitialized;
        [SerializeField] private bool autoInitializeOnAwake = true;

        public event Action<BootstrapState> OnStateChanged;
        public event Action OnBootstrapComplete;

        public BootstrapState CurrentState => currentState;
        public ReryCityIdentityConfig IdentityConfig => identityConfig;
        public bool IsReady => currentState == BootstrapState.Ready;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoInitializeOnAwake)
            {
                ExecuteOfflineBootstrap();
            }
        }

        public void ExecuteOfflineBootstrap()
        {
            SetState(BootstrapState.VerifyingLocalEnvironment);

            // Validação de configuração
            if (identityConfig == null)
            {
                identityConfig = ScriptableObject.CreateInstance<ReryCityIdentityConfig>();
            }

            SetState(BootstrapState.LoadingLocalData);

            // Carregamento local persistente
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.LoadGame();
            }

            SetState(BootstrapState.InitializingCoreSubsystems);

            // Garantir taxa de atualização de física e configurações offline
            Time.fixedDeltaTime = 0.02f;

            SetState(BootstrapState.Ready);
            OnBootstrapComplete?.Invoke();
        }

        private void SetState(BootstrapState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }
    }
}
