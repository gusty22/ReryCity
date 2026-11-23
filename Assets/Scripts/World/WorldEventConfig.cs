using System;
using UnityEngine;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Configuração e parâmetros de um tipo de ocorrência do mundo urbano.
    /// Define duração, prioridade de atendimento, raio de impacto e comportamento na via.
    /// </summary>
    [CreateAssetMenu(fileName = "New_WorldEventConfig", menuName = "Projeto Grau 2/World/World Event Config")]
    public class WorldEventConfig : ScriptableObject
    {
        [Header("Classificação e Identificação")]
        public WorldEventType eventType = WorldEventType.TrafficAccident;
        public string eventName = "Acidente de Trânsito";

        [Header("Prioridade e Escalonamento")]
        [Tooltip("Prioridade do evento (1 = Baixa/Pane, 3 = Média/Obra, 5 = Crítica/Acidente Grave).")]
        [Range(1, 5)] public int priority = 3;

        [Header("Duração e Área de Influência")]
        [Tooltip("Duração base em que o evento permanece ativo no mundo (segundos).")]
        [Range(20f, 300f)] public float baseDuration = 60f;

        [Tooltip("Raio espacial de influência do evento em metros.")]
        [Range(10f, 60f)] public float influenceRadius = 25f;

        [Tooltip("Velocidade recomendada/reduzida para veículos que entram no raio de influência (km/h).")]
        [Range(10f, 40f)] public float speedLimitInInfluenceArea = 20f;

        [Header("Intervenção Viária e Policial")]
        [Tooltip("Indica se o evento bloqueia totalmente a faixa onde ocorre.")]
        public bool blocksEntireLane = true;

        [Tooltip("Indica se a ocorrência gera um chamado de despacho para a polícia.")]
        public bool requiresPoliceDispatch = true;

        [Tooltip("Distância mínima entre este evento e outros eventos ativos (metros).")]
        [Range(30f, 150f)] public float minSpacingFromOtherEvents = 60f;
    }
}
