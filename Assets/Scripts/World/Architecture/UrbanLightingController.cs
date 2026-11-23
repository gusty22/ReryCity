using System;
using UnityEngine;
using ProjetoGrau2.Performance;

namespace ProjetoGrau2.World.Architecture
{
    /// <summary>
    /// Controlador central de iluminação pública e ambientação urbana de RERY CITY.
    /// Gerencia o acendimento noturno dos postes e adapta a fidelidade gráfica
    /// aos perfis de energia mobile (Econômico, Equilibrado e Alto Desempenho).
    /// </summary>
    [AddComponentMenu("Rery City/Architecture/Urban Lighting Controller")]
    public class UrbanLightingController : MonoBehaviour
    {
        public static UrbanLightingController Instance { get; private set; }

        [Header("Ciclo e Estado de Iluminação")]
        [SerializeField] private bool isNightTime = false;
        [SerializeField] [Range(0f, 24f)] private float timeOfDayHours = 14.0f; // 14h padrão
        [SerializeField] private float nightStartHour = 18.5f;
        [SerializeField] private float nightEndHour = 5.5f;

        [Header("Perfis de Iluminação")]
        [SerializeField] private bool allowDynamicStreetLights = true;
        [SerializeField] private float streetLightIntensity = 1.2f;
        [SerializeField] private float streetLightRange = 15f;

        public event Action<bool> OnStreetLightsToggled;

        public bool IsNightTime => isNightTime;
        public float TimeOfDayHours => timeOfDayHours;
        public bool AllowDynamicStreetLights => allowDynamicStreetLights;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SubscribeToEnergyProfiles();
            UpdateTimeOfDay(timeOfDayHours);
        }

        private void OnDestroy()
        {
            if (MobileEnergyProfileManager.Instance != null)
            {
                MobileEnergyProfileManager.Instance.OnProfileChanged -= HandleProfileChanged;
            }
        }

        private void SubscribeToEnergyProfiles()
        {
            if (MobileEnergyProfileManager.Instance != null)
            {
                MobileEnergyProfileManager.Instance.OnProfileChanged += HandleProfileChanged;
                ApplyProfileSettings(MobileEnergyProfileManager.Instance.ActiveMode);
            }
        }

        private void HandleProfileChanged(EnergyProfileMode mode, EnergyProfileSettings settings)
        {
            ApplyProfileSettings(mode);
        }

        public void ApplyProfileSettings(EnergyProfileMode mode)
        {
            switch (mode)
            {
                case EnergyProfileMode.Economico:
                    allowDynamicStreetLights = false;
                    streetLightRange = 0f;
                    break;
                case EnergyProfileMode.AltoDesempenho:
                    allowDynamicStreetLights = true;
                    streetLightRange = 22f;
                    break;
                case EnergyProfileMode.Equilibrado:
                default:
                    allowDynamicStreetLights = true;
                    streetLightRange = 14f;
                    break;
            }
        }

        /// <summary>
        /// Atualiza o horário do dia e comuta o estado das luzes públicas.
        /// </summary>
        public void UpdateTimeOfDay(float hours)
        {
            timeOfDayHours = Mathf.Repeat(hours, 24f);
            bool shouldBeNight = timeOfDayHours >= nightStartHour || timeOfDayHours <= nightEndHour;

            if (shouldBeNight != isNightTime)
            {
                isNightTime = shouldBeNight;
                OnStreetLightsToggled?.Invoke(isNightTime);
            }
        }
    }
}
