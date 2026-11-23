using System;
using UnityEngine;
using UnityEngine.UI;
using ProjetoGrau2.World;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.UI
{
    /// <summary>
    /// Componente de interface para exibição de avisos e notificações contextuais de ocorrências do mundo.
    /// Monitora o WorldEventManager e apresenta banners com título, descrição e distância em relação ao jogador.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/UI/World Event Notification UI")]
    public class WorldEventNotificationUI : MonoBehaviour
    {
        public static WorldEventNotificationUI Instance { get; private set; }

        [Header("Elementos de UI")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private Text eventTitleText;
        [SerializeField] private Text eventDescriptionText;
        [SerializeField] private Text eventDistanceText;
        [SerializeField] private Image eventIcon;

        [Header("Configurações")]
        [Range(2f, 8f)] public float displayDuration = 5.0f;
        [Range(30f, 200f)] public float maxAlertDistance = 120f;

        [Header("Referências")]
        [SerializeField] private MotorcycleController playerMotorcycle;

        private float _displayTimer = 0f;
        private string _lastActiveEventId = "";

        public bool IsNotificationActive => notificationPanel != null && notificationPanel.activeSelf;
        public string LastDisplayedTitle => (eventTitleText != null) ? eventTitleText.text : "";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (playerMotorcycle == null)
            {
                playerMotorcycle = FindFirstObjectByType<MotorcycleController>();
            }

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (WorldEventManager.Instance != null)
            {
                WorldEventManager.Instance.OnEventStarted += HandleEventStarted;
                WorldEventManager.Instance.OnEventFinished += HandleEventFinished;
            }
        }

        private void UnsubscribeEvents()
        {
            if (WorldEventManager.Instance != null)
            {
                WorldEventManager.Instance.OnEventStarted -= HandleEventStarted;
                WorldEventManager.Instance.OnEventFinished -= HandleEventFinished;
            }
        }

        private void Update()
        {
            if (_displayTimer > 0f)
            {
                _displayTimer -= Time.deltaTime;
                if (_displayTimer <= 0f)
                {
                    HideNotification();
                }
            }
        }

        private void HandleEventStarted(WorldEventInstance instance)
        {
            if (instance == null || instance.eventConfig == null) return;

            Vector3 playerPos = (playerMotorcycle != null) ? playerMotorcycle.transform.position : Vector3.zero;
            float dist = Vector3.Distance(playerPos, instance.Position);

            if (dist > maxAlertDistance) return;

            ShowEventNotification(instance.eventConfig.eventType, dist);
        }

        private void HandleEventFinished(WorldEventInstance instance)
        {
            if (instance != null && IsNotificationActive)
            {
                // Esconde a notificação se for o evento corrente
                HideNotification();
            }
        }

        public void ShowEventNotification(WorldEventType type, float distanceMeters)
        {
            string title = "";
            string desc = "";

            switch (type)
            {
                case WorldEventType.TrafficAccident:
                    title = "⚠ ACIDENTE DE TRÂNSITO";
                    desc = "Colisão com bloqueio de faixa. Reduza a velocidade e tome cuidado.";
                    break;

                case WorldEventType.RoadworkBlockage:
                    title = "🚧 OBRAS NA PISTA";
                    desc = "Interdição parcial com sinalização de cones. Desvio à frente.";
                    break;

                case WorldEventType.BrokenVehicle:
                    title = "⚠ VEÍCULO EM PANE";
                    desc = "Veículo parado na via com pisca-alerta ligado.";
                    break;

                case WorldEventType.PoliceCheckpoint:
                    title = "🚨 BLITZ POLICIAL";
                    desc = "Operação de fiscalização ostensiva. Mantenha os documentos em dia.";
                    break;

                default:
                    title = "ℹ OCORRÊNCIA VIÁRIA";
                    desc = "Atenção ao tráfego urbano.";
                    break;
            }

            int distInt = Mathf.Max(0, Mathf.RoundToInt(distanceMeters));

            if (eventTitleText != null) eventTitleText.text = title;
            if (eventDescriptionText != null) eventDescriptionText.text = desc;
            if (eventDistanceText != null) eventDistanceText.text = "A " + distInt.ToString() + "m DE VOCÊ";

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(true);
            }

            _displayTimer = displayDuration;
        }

        public void HideNotification()
        {
            _displayTimer = 0f;
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }
    }
}
