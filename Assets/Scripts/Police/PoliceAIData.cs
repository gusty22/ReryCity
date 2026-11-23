using System;
using UnityEngine;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Configuração e parâmetros de inteligência artificial de perseguição policial.
    /// Define limites de visão, audição, critérios de abordagem/prisão e tolerâncias de perda de alvo.
    /// </summary>
    [CreateAssetMenu(fileName = "New_PoliceAIData", menuName = "Projeto Grau 2/Police/Police AI Data")]
    public class PoliceAIData : ScriptableObject
    {
        [Header("Percepção Sensorial")]
        [Tooltip("Alcance máximo de visão direta da guarnição (metros).")]
        [Range(20f, 100f)] public float visionRange = 55f;

        [Tooltip("Ângulo horizontal do cone de visão frontal (graus).")]
        [Range(60f, 160f)] public float visionFieldOfView = 110f;

        [Tooltip("Raio de percepção auditiva (escapamento aberto, corte de giro, arrancadas bruscas).")]
        [Range(10f, 60f)] public float hearingRange = 35f;

        [Header("Táticas de Perseguição")]
        [Tooltip("Distância desejada de acompanhamento em perseguição direta (metros).")]
        [Range(3f, 20f)] public float pursuitFollowDistance = 8f;

        [Tooltip("Tempo de predição linear da rota do jogador para manobras de interceptação (segundos).")]
        [Range(0.5f, 4.0f)] public float interceptLookAheadTime = 1.8f;

        [Tooltip("Distância lateral para fechamento / bloqueio suave de passagem.")]
        [Range(1.5f, 5.0f)] public float interceptLateralMargin = 2.5f;

        [Header("Critérios de Prisão e Abordagem")]
        [Tooltip("Distância máxima para iniciar contagem regressiva de abordagem/prisão (metros).")]
        [Range(2f, 8f)] public float arrestDistance = 4.5f;

        [Tooltip("Tempo contínuo que o infrator deve permanecer imobilizado ou cercado para consumar a prisão (segundos).")]
        [Range(1.5f, 6.0f)] public float arrestDurationRequired = 3.0f;

        [Tooltip("Velocidade máxima da moto do jogador (km/h) para ser considerada imobilizada na abordagem.")]
        [Range(2f, 15f)] public float playerMaxArrestSpeedKmh = 8f;

        [Header("Perda de Contato e Busca")]
        [Tooltip("Tempo que a viatura investiga a última posição conhecida após perder o contato visual (segundos).")]
        [Range(4f, 25f)] public float lostTargetTimeout = 12f;

        [Tooltip("Raio de patrulhamento ao redor da última posição conhecida (metros).")]
        [Range(15f, 60f)] public float searchRadius = 30f;
    }
}
