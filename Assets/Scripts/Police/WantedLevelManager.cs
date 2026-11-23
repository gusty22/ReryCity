using System;
using UnityEngine;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Gerenciador central do nível de procurado (Wanted Level) de 0 a 5 estrelas.
    /// Acumula pontos de infração, governa o escalonamento de resposta policial e calcula
    /// a taxa de evasão e esquecimento quando o jogador despista as viaturas.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Wanted Level Manager")]
    public class WantedLevelManager : MonoBehaviour
    {
        public static WantedLevelManager Instance { get; private set; }

        [Header("Estado do Nível de Procurado")]
        [Range(0, 5)] [SerializeField] private int currentWantedLevel = 0;
        [SerializeField] private int currentViolationScore = 0;

        [Header("Limiares de Pontos para Subir de Nível")]
        public int thresholdLevel1 = 20;  // Infração identificada
        public int thresholdLevel2 = 60;  // Acompanhamento policial
        public int thresholdLevel3 = 130; // Perseguição ativa
        public int thresholdLevel4 = 250; // Reforços e bloqueios
        public int thresholdLevel5 = 450; // Cerco tático máximo

        [Header("Evasão e Esquecimento (Decay)")]
        [Tooltip("Tempo necessário fora da vista de qualquer viatura para reduzir 1 estrela (segundos).")]
        [Range(5f, 40f)] public float evasionDurationPerStar = 15f;

        [SerializeField] private bool isPlayerSpotted = false;
        [SerializeField] private float evasionTimer = 0f;

        // Eventos públicos
        public event Action<int, int> OnWantedLevelChanged;
        public event Action<int, string> OnScoreAdded;
        public event Action<bool> OnPlayerSpottedChanged;
        public event Action<float> OnEvasionProgressUpdated; // 0.0 a 1.0

        public int CurrentWantedLevel => currentWantedLevel;
        public int CurrentViolationScore => currentViolationScore;
        public bool IsPlayerSpotted => isPlayerSpotted;
        public bool IsWanted => currentWantedLevel > 0;
        public float EvasionProgress => (currentWantedLevel > 0 && !isPlayerSpotted) ? Mathf.Clamp01(evasionTimer / evasionDurationPerStar) : 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            UpdateEvasion(Time.deltaTime);
        }

        /// <summary>
        /// Acumula pontuação de infração e avalia elevação do nível de procurado.
        /// </summary>
        public void AddViolationScore(int points, string reason = "")
        {
            if (points <= 0) return;

            currentViolationScore += points;
            OnScoreAdded?.Invoke(points, reason);

            // Ao cometer nova infração visível, reseta timer de evasão
            evasionTimer = 0f;

            EvaluateWantedLevelUpgrade();
        }

        private void EvaluateWantedLevelUpgrade()
        {
            int oldLevel = currentWantedLevel;
            int newLevel = 0;

            if (currentViolationScore >= thresholdLevel5) newLevel = 5;
            else if (currentViolationScore >= thresholdLevel4) newLevel = 4;
            else if (currentViolationScore >= thresholdLevel3) newLevel = 3;
            else if (currentViolationScore >= thresholdLevel2) newLevel = 2;
            else if (currentViolationScore >= thresholdLevel1) newLevel = 1;

            if (newLevel > currentWantedLevel)
            {
                SetWantedLevel(newLevel);
            }
        }

        /// <summary>
        /// Define o nível de procurado forçadamente ou por escalonamento.
        /// </summary>
        public void SetWantedLevel(int level)
        {
            level = Mathf.Clamp(level, 0, 5);
            if (currentWantedLevel == level) return;

            int oldLevel = currentWantedLevel;
            currentWantedLevel = level;
            evasionTimer = 0f;

            OnWantedLevelChanged?.Invoke(oldLevel, currentWantedLevel);
        }

        /// <summary>
        /// Notifica se ao menos uma viatura/ROCAM possui linha de visão clara com o jogador.
        /// </summary>
        public void SetPlayerSpotted(bool spotted)
        {
            if (isPlayerSpotted != spotted)
            {
                isPlayerSpotted = spotted;
                if (isPlayerSpotted)
                {
                    evasionTimer = 0f;
                }
                OnPlayerSpottedChanged?.Invoke(isPlayerSpotted);
            }
        }

        private void UpdateEvasion(float dt)
        {
            if (currentWantedLevel == 0) return;

            if (!isPlayerSpotted)
            {
                evasionTimer += dt;
                float progress = Mathf.Clamp01(evasionTimer / evasionDurationPerStar);
                OnEvasionProgressUpdated?.Invoke(progress);

                if (evasionTimer >= evasionDurationPerStar)
                {
                    evasionTimer = 0f;
                    // Reduz um nível de procurado
                    int newLevel = currentWantedLevel - 1;
                    SetWantedLevel(newLevel);

                    // Ajusta pontuação para o piso do novo nível
                    currentViolationScore = GetThresholdForLevel(newLevel);
                }
            }
            else
            {
                evasionTimer = 0f;
                OnEvasionProgressUpdated?.Invoke(0f);
            }
        }

        private int GetThresholdForLevel(int level)
        {
            return level switch
            {
                5 => thresholdLevel5,
                4 => thresholdLevel4,
                3 => thresholdLevel3,
                2 => thresholdLevel2,
                1 => thresholdLevel1,
                _ => 0
            };
        }

        /// <summary>
        /// Reseta integralmente o estado de procurado (ex: após prisão consumada ou restart).
        /// </summary>
        public void ClearWantedLevel()
        {
            currentViolationScore = 0;
            evasionTimer = 0f;
            isPlayerSpotted = false;
            SetWantedLevel(0);
        }
    }
}
