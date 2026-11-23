using System;
using UnityEngine;

namespace ProjetoGrau2.Performance
{
    public enum EnergyProfileMode
    {
        Economico,
        Equilibrado,
        AltoDesempenho
    }

    [System.Serializable]
    public struct EnergyProfileSettings
    {
        public EnergyProfileMode mode;
        public int targetFrameRate;
        public float lodBias;
        public int vSyncCount;
        public ShadowQuality shadowQuality;
        public float shadowDistance;
        public bool limitSleepTimeout;

        public static EnergyProfileSettings GetDefault(EnergyProfileMode mode)
        {
            switch (mode)
            {
                case EnergyProfileMode.Economico:
                    return new EnergyProfileSettings
                    {
                        mode = EnergyProfileMode.Economico,
                        targetFrameRate = 30,
                        lodBias = 0.7f,
                        vSyncCount = 0,
                        shadowQuality = ShadowQuality.Disable,
                        shadowDistance = 25f,
                        limitSleepTimeout = false
                    };
                case EnergyProfileMode.AltoDesempenho:
                    return new EnergyProfileSettings
                    {
                        mode = EnergyProfileMode.AltoDesempenho,
                        targetFrameRate = 60,
                        lodBias = 1.5f,
                        vSyncCount = 0,
                        shadowQuality = ShadowQuality.All,
                        shadowDistance = 80f,
                        limitSleepTimeout = true
                    };
                case EnergyProfileMode.Equilibrado:
                default:
                    return new EnergyProfileSettings
                    {
                        mode = EnergyProfileMode.Equilibrado,
                        targetFrameRate = 60,
                        lodBias = 1.0f,
                        vSyncCount = 0,
                        shadowQuality = ShadowQuality.HardOnly,
                        shadowDistance = 50f,
                        limitSleepTimeout = true
                    };
            }
        }
    }

    /// <summary>
    /// Gerenciador de perfis de energia e desempenho mobile para RERY CITY.
    /// Permite alternar dinamicamente entre perfis Econômico, Equilibrado e Alto Desempenho,
    /// otimizando consumo de bateria, taxa de quadros e estabilidade térmica em dispositivos móveis.
    /// </summary>
    [AddComponentMenu("Rery City/Performance/Mobile Energy Profile Manager")]
    public class MobileEnergyProfileManager : MonoBehaviour
    {
        public static MobileEnergyProfileManager Instance { get; private set; }

        private const string PREF_KEY_PROFILE = "ReryCity_EnergyProfile";

        [Header("Configuração Atual")]
        [SerializeField] private EnergyProfileMode activeMode = EnergyProfileMode.Equilibrado;
        [SerializeField] private bool applyOnAwake = true;

        [Header("Definições dos Perfis")]
        [SerializeField] private EnergyProfileSettings economicoSettings = EnergyProfileSettings.GetDefault(EnergyProfileMode.Economico);
        [SerializeField] private EnergyProfileSettings equilibradoSettings = EnergyProfileSettings.GetDefault(EnergyProfileMode.Equilibrado);
        [SerializeField] private EnergyProfileSettings altoDesempenhoSettings = EnergyProfileSettings.GetDefault(EnergyProfileMode.AltoDesempenho);

        public event Action<EnergyProfileMode, EnergyProfileSettings> OnProfileChanged;

        public EnergyProfileMode ActiveMode => activeMode;
        public EnergyProfileSettings CurrentSettings => GetSettingsForMode(activeMode);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSavedProfile();

            if (applyOnAwake)
            {
                ApplyProfile(activeMode);
            }
        }

        public void SetProfile(EnergyProfileMode mode)
        {
            activeMode = mode;
            ApplyProfile(mode);
            SaveCurrentProfile();
        }

        public void ApplyProfile(EnergyProfileMode mode)
        {
            EnergyProfileSettings settings = GetSettingsForMode(mode);

            Application.targetFrameRate = settings.targetFrameRate;
            QualitySettings.vSyncCount = settings.vSyncCount;
            QualitySettings.lodBias = settings.lodBias;
            QualitySettings.shadows = settings.shadowQuality;
            QualitySettings.shadowDistance = settings.shadowDistance;

            if (settings.limitSleepTimeout)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }

            OnProfileChanged?.Invoke(mode, settings);
        }

        public EnergyProfileSettings GetSettingsForMode(EnergyProfileMode mode)
        {
            switch (mode)
            {
                case EnergyProfileMode.Economico:
                    return economicoSettings;
                case EnergyProfileMode.AltoDesempenho:
                    return altoDesempenhoSettings;
                case EnergyProfileMode.Equilibrado:
                default:
                    return equilibradoSettings;
            }
        }

        private void SaveCurrentProfile()
        {
            PlayerPrefs.SetInt(PREF_KEY_PROFILE, (int)activeMode);
            PlayerPrefs.Save();
        }

        private void LoadSavedProfile()
        {
            if (PlayerPrefs.HasKey(PREF_KEY_PROFILE))
            {
                int saved = PlayerPrefs.GetInt(PREF_KEY_PROFILE);
                if (Enum.IsDefined(typeof(EnergyProfileMode), saved))
                {
                    activeMode = (EnergyProfileMode)saved;
                }
            }
        }
    }
}
