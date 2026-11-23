using UnityEngine;

namespace ProjetoGrau2.Performance
{
    /// <summary>
    /// Configuração persistente e editável de perfis de energia para RERY CITY.
    /// </summary>
    [CreateAssetMenu(fileName = "MobileEnergyProfile_Default", menuName = "Rery City/Performance/Energy Profile Config")]
    public class MobileEnergyProfileConfig : ScriptableObject
    {
        [Header("Perfis Padrão")]
        public EnergyProfileSettings economico = EnergyProfileSettings.GetDefault(EnergyProfileMode.Economico);
        public EnergyProfileSettings equilibrado = EnergyProfileSettings.GetDefault(EnergyProfileMode.Equilibrado);
        public EnergyProfileSettings altoDesempenho = EnergyProfileSettings.GetDefault(EnergyProfileMode.AltoDesempenho);
    }
}
