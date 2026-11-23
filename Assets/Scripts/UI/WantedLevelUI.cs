using System;
using UnityEngine;
using UnityEngine.UI;
using ProjetoGrau2.Police;

namespace ProjetoGrau2.UI
{
    /// <summary>
    /// Componente de interface de usuário para exibição do nível de procurado (Estrelas).
    /// Gerencia a exibição visual de 1 a 5 estrelas, pisca em cinza/amarelo durante o período de evasão
    /// e desaparece quando o nível retorna a zero.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/UI/Wanted Level UI")]
    public class WantedLevelUI : MonoBehaviour
    {
        [Header("Elementos de UI")]
        [Tooltip("Array contendo os 5 elementos de estrela (Image ou GameObject).")]
        [SerializeField] private Image[] starImages = new Image[5];

        [Header("Cores dos Estados")]
        public Color activeStarColor = new Color(1f, 0.85f, 0.1f, 1f);   // Dourado
        public Color inactiveStarColor = new Color(0.2f, 0.2f, 0.2f, 0.4f); // Translúcido
        public Color evadingStarColor = new Color(0.9f, 0.9f, 0.9f, 0.8f);  // Branco/Cinza piscante

        private bool _isSpotted = false;
        private int _currentLevel = 0;
        private float _blinkTimer = 0f;

        private void Start()
        {
            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.OnWantedLevelChanged += HandleWantedLevelChanged;
                WantedLevelManager.Instance.OnPlayerSpottedChanged += HandlePlayerSpottedChanged;
                UpdateStarsDisplay(WantedLevelManager.Instance.CurrentWantedLevel);
            }
            else
            {
                UpdateStarsDisplay(0);
            }
        }

        private void OnDestroy()
        {
            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.OnWantedLevelChanged -= HandleWantedLevelChanged;
                WantedLevelManager.Instance.OnPlayerSpottedChanged -= HandlePlayerSpottedChanged;
            }
        }

        private void Update()
        {
            if (_currentLevel > 0 && !_isSpotted)
            {
                // Pisca as estrelas ativas durante evasão
                _blinkTimer += Time.deltaTime * 4.0f;
                bool toggle = ((int)_blinkTimer % 2) == 0;
                ApplyBlinkState(toggle);
            }
        }

        private void HandleWantedLevelChanged(int oldLevel, int newLevel)
        {
            _currentLevel = newLevel;
            UpdateStarsDisplay(newLevel);
        }

        private void HandlePlayerSpottedChanged(bool spotted)
        {
            _isSpotted = spotted;
            if (_isSpotted)
            {
                UpdateStarsDisplay(_currentLevel);
            }
        }

        public void UpdateStarsDisplay(int level)
        {
            _currentLevel = level;
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;

                if (i < level)
                {
                    starImages[i].enabled = true;
                    starImages[i].color = activeStarColor;
                }
                else
                {
                    starImages[i].enabled = (level > 0);
                    starImages[i].color = inactiveStarColor;
                }
            }
        }

        private void ApplyBlinkState(bool visible)
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;

                if (i < _currentLevel)
                {
                    starImages[i].color = visible ? activeStarColor : evadingStarColor;
                }
            }
        }
    }
}
