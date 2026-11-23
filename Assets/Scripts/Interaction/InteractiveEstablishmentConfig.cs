using System;
using UnityEngine;

namespace ProjetoGrau2.Interaction
{
    public enum EstablishmentCategory
    {
        Workshop,         // Oficina Mecânica ("Oficina do Grau")
        GasStation,       // Posto de Combustíveis ("Posto Pioneiro")
        TireShop,         // Borracharia
        ConvenienceStore, // Mercadinho / Loja de Conveniência
        AutoParts,        // Autopeças
        Landmark          // Ponto Turístico / Histórico de Araguari
    }

    /// <summary>
    /// Configuração ScriptableObject para estabelecimentos urbanos interativos em RERY CITY.
    /// Define metadados comerciais, localização, horário de funcionamento e preços de serviços.
    /// </summary>
    [CreateAssetMenu(fileName = "EstablishmentConfig_New", menuName = "Rery City/Interaction/Establishment Config")]
    public class InteractiveEstablishmentConfig : ScriptableObject
    {
        [Header("Identificação do Estabelecimento")]
        public string establishmentId = "est_oficina_grau_01";
        public string establishmentName = "Oficina do Grau & Performance";
        public EstablishmentCategory category = EstablishmentCategory.Workshop;
        public string neighborhoodName = "Centro";

        [Header("Geolocalização Urbana")]
        public Vector3 worldPosition = Vector3.zero;
        public float interactionRadius = 4.0f;

        [Header("Tabela de Serviços e Economia")]
        public int fuelPricePerLiterCentavos = 589;    // R$ 5,89/L
        public int repairCostPerPercentCentavos = 1500; // R$ 15,00 por % de dano
        public int tireMaintenanceCostCentavos = 2500;  // R$ 25,00 troca/calibragem
        public string[] availableServices = new string[] { "Troca de Óleo", "Regulagem de Válvulas", "Instalação ECU Athlon", "Revisão Geral" };

        [Header("Horário de Funcionamento")]
        public bool isOpen24Hours = true;
        [Range(0f, 24f)] public float openHour = 7.0f;
        [Range(0f, 24f)] public float closeHour = 20.0f;

        public bool IsCurrentlyOpen(float currentHour)
        {
            if (isOpen24Hours) return true;
            if (openHour <= closeHour)
            {
                return currentHour >= openHour && currentHour < closeHour;
            }
            // Turno noturno que cruza meia-noite
            return currentHour >= openHour || currentHour < closeHour;
        }
    }
}
