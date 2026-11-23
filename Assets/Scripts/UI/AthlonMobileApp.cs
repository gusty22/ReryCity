using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Propulsion;

namespace ProjetoGrau2.UI
{
    [Serializable]
    public struct AthlonMapProfile
    {
        public string profileName;
        public float mainCutRpm;
        public float hysteresisRpm;
        public bool twoStepActive;
        public float twoStepRpm;
        public EcuCutMode cutMode;
        public float backfireProbability;
    }

    /// <summary>
    /// Aplicativo de celular virtual integrado à ECU Athlon.
    /// Permite calibração em tempo real dos mapas de corte, Two-Step, histerese e estampidos,
    /// com suporte nativo a touchscreen mobile e teclado/mouse PC.
    /// </summary>
    public class AthlonMobileApp : MonoBehaviour
    {
        [Header("Referência da ECU")]
        [SerializeField] private AthlonECUController ecuController;

        [Header("Interface e Visibilidade")]
        [SerializeField] private bool isPhoneOpen = false;
        [SerializeField] private KeyCode togglePhoneKey = KeyCode.M;

        // Perfis salvos em memória
        private readonly List<AthlonMapProfile> savedProfiles = new List<AthlonMapProfile>();
        private int selectedProfileIndex = 0;

        // Estilos de GUI
        private GUIStyle phoneFrameStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle activeTabStyle;
        private bool stylesInitialized = false;

        public bool IsPhoneOpen => isPhoneOpen;

        private void Awake()
        {
            if (ecuController == null)
            {
                ecuController = FindFirstObjectByType<AthlonECUController>();
            }

            InitDefaultProfiles();
        }

        private void InitDefaultProfiles()
        {
            savedProfiles.Add(new AthlonMapProfile
            {
                profileName = "Mapa 1: Original Seguro",
                mainCutRpm = 9800f,
                hysteresisRpm = 180f,
                twoStepActive = true,
                twoStepRpm = 5000f,
                cutMode = EcuCutMode.IgnitionCut,
                backfireProbability = 0.4f
            });

            savedProfiles.Add(new AthlonMapProfile
            {
                profileName = "Mapa 2: Manobras & Grau",
                mainCutRpm = 10400f,
                hysteresisRpm = 220f,
                twoStepActive = true,
                twoStepRpm = 5400f,
                cutMode = EcuCutMode.IgnitionCut,
                backfireProbability = 0.85f
            });

            savedProfiles.Add(new AthlonMapProfile
            {
                profileName = "Mapa 3: Metralhadora Seca",
                mainCutRpm = 10600f,
                hysteresisRpm = 120f,
                twoStepActive = false,
                twoStepRpm = 5200f,
                cutMode = EcuCutMode.Both,
                backfireProbability = 0.95f
            });
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(togglePhoneKey))
            {
                TogglePhone();
            }
        }

        public void TogglePhone()
        {
            isPhoneOpen = !isPhoneOpen;
        }

        public void SetPhoneOpen(bool open)
        {
            isPhoneOpen = open;
        }

        private void InitStyles()
        {
            if (stylesInitialized) return;

            phoneFrameStyle = new GUIStyle(GUI.skin.box);

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.1f, 0.9f, 1f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = Color.white }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11
            };

            activeTabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            // Botão discreto para abrir celular se fechado
            if (!isPhoneOpen)
            {
                if (GUI.Button(new Rect(Screen.width - 130, Screen.height - 40, 120, 30), "📱 Celular Athlon"))
                {
                    TogglePhone();
                }
                return;
            }

            InitStyles();

            // Moldura do Celular no canto inferior direito
            float phoneW = 300f;
            float phoneH = 460f;
            float phoneX = Screen.width - phoneW - 15f;
            float phoneY = Screen.height - phoneH - 15f;

            GUI.color = new Color(0.08f, 0.08f, 0.12f, 0.96f);
            GUI.Box(new Rect(phoneX, phoneY, phoneW, phoneH), GUIContent.none);
            GUI.color = Color.white;

            GUILayout.BeginArea(new Rect(phoneX + 12, phoneY + 10, phoneW - 24, phoneH - 20));

            // Top Bar do Celular
            GUILayout.BeginHorizontal();
            GUILayout.Label("📱 Athlon Smart ECU", titleStyle);
            if (GUILayout.Button("✕", GUILayout.Width(25), GUILayout.Height(20)))
            {
                TogglePhone();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("<color=#33FF88>● Conectado via Bluetooth (160cc)</color>", labelStyle);
            GUILayout.Space(6);

            if (ecuController != null && ecuController.Data != null)
            {
                AthlonECUData data = ecuController.Data;

                // 1. Limitador Principal
                GUILayout.Label($"Limitador de Giro (Hard Cut): {data.mainCutRpm:F0} RPM", labelStyle);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("- 100", buttonStyle)) data.mainCutRpm = Mathf.Max(7000f, data.mainCutRpm - 100f);
                if (GUILayout.Button("+ 100", buttonStyle)) data.mainCutRpm = Mathf.Min(12500f, data.mainCutRpm + 100f);
                GUILayout.EndHorizontal();

                GUILayout.Space(4);

                // 2. Controle de Largada (Two-Step)
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Two-Step: {(data.twoStepActive ? "<color=green>LIGADO</color>" : "<color=gray>DESLIGADO</color>")}", labelStyle);
                if (GUILayout.Button(data.twoStepActive ? "Desativar" : "Ativar", GUILayout.Width(80)))
                {
                    data.twoStepActive = !data.twoStepActive;
                }
                GUILayout.EndHorizontal();

                if (data.twoStepActive)
                {
                    GUILayout.Label($"Rotação Two-Step: {data.twoStepRpm:F0} RPM", labelStyle);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("- 100", buttonStyle)) data.twoStepRpm = Mathf.Max(3500f, data.twoStepRpm - 100f);
                    if (GUILayout.Button("+ 100", buttonStyle)) data.twoStepRpm = Mathf.Min(8000f, data.twoStepRpm + 100f);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(4);

                // 3. Histerese do Corte
                GUILayout.Label($"Histerese de Religue: {data.hysteresisRpm:F0} RPM", labelStyle);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cadência Lenta", buttonStyle)) data.hysteresisRpm = 350f;
                if (GUILayout.Button("Média", buttonStyle)) data.hysteresisRpm = 200f;
                if (GUILayout.Button("Rápida", buttonStyle)) data.hysteresisRpm = 100f;
                GUILayout.EndHorizontal();

                GUILayout.Space(4);

                // 4. Tipo de Intervenção
                GUILayout.Label($"Tipo de Corte: {data.cutMode}", labelStyle);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Ignição", data.cutMode == EcuCutMode.IgnitionCut ? activeTabStyle : buttonStyle)) data.cutMode = EcuCutMode.IgnitionCut;
                if (GUILayout.Button("Injeção", data.cutMode == EcuCutMode.FuelCut ? activeTabStyle : buttonStyle)) data.cutMode = EcuCutMode.FuelCut;
                if (GUILayout.Button("Ambos", data.cutMode == EcuCutMode.Both ? activeTabStyle : buttonStyle)) data.cutMode = EcuCutMode.Both;
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                // 5. Seleção e Salvamento de Mapas
                GUILayout.Label("── MAPAS PROGRAMADOS ──", labelStyle);
                for (int i = 0; i < savedProfiles.Count; i++)
                {
                    if (GUILayout.Button(savedProfiles[i].profileName, (i == selectedProfileIndex) ? activeTabStyle : buttonStyle))
                    {
                        selectedProfileIndex = i;
                        LoadProfile(savedProfiles[i]);
                    }
                }
            }
            else
            {
                GUILayout.Label("<color=red>Aguardando sincronização com a ECU...</color>", labelStyle);
            }

            GUILayout.EndArea();
        }

        private void LoadProfile(AthlonMapProfile profile)
        {
            if (ecuController != null && ecuController.Data != null)
            {
                var data = ecuController.Data;
                data.mainCutRpm = profile.mainCutRpm;
                data.hysteresisRpm = profile.hysteresisRpm;
                data.twoStepActive = profile.twoStepActive;
                data.twoStepRpm = profile.twoStepRpm;
                data.cutMode = profile.cutMode;
                data.backfireProbability = profile.backfireProbability;
            }
        }
    }
}
