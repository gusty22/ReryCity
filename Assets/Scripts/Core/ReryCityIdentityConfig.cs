using UnityEngine;

namespace ProjetoGrau2.Core
{
    /// <summary>
    /// Configuração central de identidade do jogo independente RERY CITY.
    /// Define metadados públicos, versão, temática urbana e desvinculação de identidade legada.
    /// </summary>
    [CreateAssetMenu(fileName = "ReryCityIdentity_Default", menuName = "Rery City/Identity/Identity Config")]
    public class ReryCityIdentityConfig : ScriptableObject
    {
        [Header("Identidade Oficial do Produto")]
        [SerializeField] private string gameTitle = "RERY CITY";
        [SerializeField] private string publicSubtitle = "Simulador Urbano de Motocicletas e Manobras";
        [SerializeField] private string versionString = "1.0.0-indie";
        [SerializeField] private int buildNumber = 100;

        [Header("Ambientação e Cidade")]
        [SerializeField] private string settingName = "Araguari Futurista";
        [SerializeField] private string settingDescription = "Representação urbana inspirada em Araguari, MG, em um cenário de liberdade e cultura do grau.";

        [Header("Diretrizes de Operação")]
        [SerializeField] private bool forceOfflineMode = true;
        [SerializeField] private bool allowSimulatedRewards = true;
        [SerializeField] private bool enableTelemetry = false;

        public string GameTitle => gameTitle;
        public string PublicSubtitle => publicSubtitle;
        public string VersionString => versionString;
        public int BuildNumber => buildNumber;
        public string SettingName => settingName;
        public string SettingDescription => settingDescription;
        public bool ForceOfflineMode => forceOfflineMode;
        public bool AllowSimulatedRewards => allowSimulatedRewards;
        public bool EnableTelemetry => enableTelemetry;

        public string GetFormattedTitleAndVersion()
        {
            return $"{gameTitle} v{versionString} (b{buildNumber})";
        }
    }
}
