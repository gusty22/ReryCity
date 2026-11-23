using UnityEngine;
using ProjetoGrau2.Physics;
using ProjetoGrau2.Core;
using ProjetoGrau2.Propulsion;

namespace ProjetoGrau2.Telemetry
{
    /// <summary>
    /// Componente de exibição de telemetria física e de propulsão em tempo real.
    /// Funciona nativamente com OnGUI (sem dependência de Canvas externa).
    /// </summary>
    public class PhysicsTelemetryUI : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private WheeliePhysicsController wheelieController;
        [SerializeField] private MotorcycleController motorcycleController;

        [Header("Configurações Visuais")]
        [SerializeField] private bool showTelemetry = true;
        [SerializeField] private bool showAngleGauge = true;

        private WheelieTelemetryData latestTelemetry;
        private GUIStyle headerStyle;
        private GUIStyle subHeaderStyle;
        private GUIStyle labelStyle;
        private GUIStyle alertStyle;
        private bool stylesInitialized = false;

        private void Awake()
        {
            if (wheelieController == null) wheelieController = GetComponent<WheeliePhysicsController>();
            if (motorcycleController == null) motorcycleController = GetComponent<MotorcycleController>();
        }

        private void OnEnable()
        {
            if (wheelieController != null)
            {
                wheelieController.OnTelemetryUpdated += HandleTelemetry;
            }
        }

        private void OnDisable()
        {
            if (wheelieController != null)
            {
                wheelieController.OnTelemetryUpdated -= HandleTelemetry;
            }
        }

        private void HandleTelemetry(WheelieTelemetryData data)
        {
            latestTelemetry = data;
        }

        private void InitStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.2f, 0.9f, 1f) }
            };

            subHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.85f, 0.2f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = Color.white }
            };

            alertStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.25f, 0.25f) }
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!showTelemetry) return;
            InitStyles();

            float speedKmh = motorcycleController != null ? motorcycleController.CurrentSpeedKmh : 0f;
            float targetBalance = (motorcycleController != null && motorcycleController.Data != null) ? motorcycleController.Data.balanceAngle : 60f;

            EngineController engine = motorcycleController != null ? motorcycleController.Engine : null;
            TransmissionController trans = motorcycleController != null ? motorcycleController.Transmission : null;
            AthlonECUController ecu = motorcycleController != null ? motorcycleController.ECU : null;

            // Painel Principal
            GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.90f);
            GUI.Box(new Rect(12, 12, 340, 360), GUIContent.none);
            GUI.color = Color.white;

            GUILayout.BeginArea(new Rect(20, 16, 324, 350));
            GUILayout.Label("══ TELEMETRIA PROJETO GRAU 2 (FASE 2) ══", headerStyle);
            GUILayout.Space(2);

            // Bloco de Propulsão
            GUILayout.Label("── PROPULSÃO & MOTOR ──", subHeaderStyle);
            float rpm = engine != null ? engine.CurrentRpm : 0f;
            int gear = trans != null ? trans.CurrentGear : 0;
            string gearText = gear == 0 ? "N" : $"{gear}ª";
            float torque = engine != null ? engine.CurrentTorqueNm : 0f;
            float power = engine != null ? engine.CurrentPowerCv : 0f;
            float throttle = engine != null ? engine.EffectiveThrottle : 0f;
            bool isCutting = (ecu != null && ecu.IsCutting) || (engine != null && engine.IsCutActive);

            GUILayout.Label($"Velocidade: {speedKmh:F1} km/h  |  Marcha: {gearText}  |  Acel: {throttle * 100:F0}%", labelStyle);
            GUILayout.Label($"RPM: {rpm:F0} RPM  |  Potência: {power:F1} CV  |  Torque: {torque:F1} Nm", labelStyle);
            GUILayout.Label($"Força Tratora: {(trans != null ? trans.TractiveForceN : 0f):F0} N  |  Embreagem: {(trans != null ? (1f - trans.ClutchEngagement) * 100 : 0f):F0}%", labelStyle);

            string ecuStatus = isCutting ? "<color=red>CORTE ATIVO (POPS & BANGS)</color>" : "<color=green>NORMAL</color>";
            if (ecu != null && ecu.IsTwoStepActive) ecuStatus = "<color=yellow>TWO-STEP (LARGADA)</color>";
            GUILayout.Label($"ECU Athlon: {ecuStatus}", labelStyle);

            GUILayout.Space(4);
            // Bloco do Grau
            GUILayout.Label("── DINÂMICA DO GRAU ──", subHeaderStyle);
            GUILayout.Label($"Ângulo Atual (Pitch): {latestTelemetry.PitchAngle:F1}°  |  Alvo: {targetBalance:F0}°", labelStyle);
            GUILayout.Label($"Vel. Angular X: {latestTelemetry.AngularVelocityX:F1}°/s  |  Tempo Grau: {latestTelemetry.TimeInWheelie:F2}s", labelStyle);
            GUILayout.Label($"Zona de Equilíbrio: {(latestTelemetry.InBalanceZone ? "<color=green>EQUILÍBRIO ATIVO</color>" : "<color=gray>FORA DA ZONA</color>")}", labelStyle);

            if (latestTelemetry.IsCrashed)
            {
                GUILayout.Space(2);
                GUILayout.Label("⚠ QUEDA DETECTADA (Pressione 'R' para Reset)", alertStyle);
            }

            GUILayout.EndArea();

            if (showAngleGauge)
            {
                DrawGauges(targetBalance, rpm, engine != null && engine.Data != null ? engine.Data.maxRpm : 11000f);
            }
        }

        private void DrawGauges(float targetBalance, float currentRpm, float maxRpm)
        {
            float barX = 12;
            float barWidth = 340;

            // 1. Barra de RPM (Tacômetro)
            float rpmY = 380;
            float rpmProgress = Mathf.Clamp01(currentRpm / maxRpm);
            GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            GUI.Box(new Rect(barX, rpmY, barWidth, 18), GUIContent.none);

            GUI.color = (rpmProgress > 0.88f) ? Color.red : ((rpmProgress > 0.65f) ? Color.yellow : Color.cyan);
            GUI.Box(new Rect(barX, rpmY, barWidth * rpmProgress, 18), GUIContent.none);

            GUI.color = Color.white;
            GUI.Label(new Rect(barX + 4, rpmY - 1, 150, 18), $"RPM: {currentRpm:F0} / {maxRpm:F0}", labelStyle);

            // 2. Barra de Ângulo (Grau)
            float angleY = 405;
            float angleProgress = Mathf.Clamp01(latestTelemetry.PitchAngle / 90f);
            float targetProgress = Mathf.Clamp01(targetBalance / 90f);

            GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            GUI.Box(new Rect(barX, angleY, barWidth, 18), GUIContent.none);

            GUI.color = latestTelemetry.InBalanceZone ? Color.green : (latestTelemetry.IsCrashed ? Color.red : new Color(0.2f, 0.8f, 1f));
            GUI.Box(new Rect(barX, angleY, barWidth * angleProgress, 18), GUIContent.none);

            GUI.color = Color.yellow;
            GUI.Box(new Rect(barX + (barWidth * targetProgress) - 1, angleY - 2, 3, 22), GUIContent.none);

            GUI.color = Color.white;
            GUI.Label(new Rect(barX + 4, angleY - 1, 100, 18), $"Pitch: {latestTelemetry.PitchAngle:F1}°", labelStyle);
            GUI.Label(new Rect(barX + barWidth - 30, angleY - 1, 60, 18), "90°", labelStyle);
        }
    }
}
