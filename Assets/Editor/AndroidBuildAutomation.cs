#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ProjetoGrau2.EditorBuild
{
    /// <summary>
    /// Script de automação para compilação e empacotamento do APK Android de RERY CITY.
    /// Suporta execução interativa via menu do Editor ou headless via linha de comando (-executeMethod).
    /// Alvo de referência: Samsung Galaxy A54 5G (ARM64, URP, offline).
    /// </summary>
    public static class AndroidBuildAutomation
    {
        private const string ScenePath = "Assets/Scenes/AraguariPilotCentroScene.unity";
        private const string BuildOutputFolder = "Builds/Android";
        private const string ApkName = "ReryCity_Araguari_Pilot_v0.1.apk";

        [MenuItem("Rery City/Build/Build Android Dev APK (Galaxy A54)")]
        public static void BuildDevApk()
        {
            Debug.Log("[AndroidBuildAutomation] Iniciando pipeline de build para Android (Galaxy A54)...");

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(projectRoot, BuildOutputFolder);
            Directory.CreateDirectory(outputDir);
            string apkPath = Path.Combine(outputDir, ApkName);

            // 1. Configurações de Identidade e Plataforma
            PlayerSettings.companyName = "Insanity Group";
            PlayerSettings.productName = "RERY CITY";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.InsanityGroup.ProjetoGrau2");
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 100;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

            // 2. Arquitetura e Compilação
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28; // Android 9.0+
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Android 14

            // 3. Opções de Build
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            Debug.Log($"[AndroidBuildAutomation] Compilando cena: {ScenePath} -> {apkPath}");
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[AndroidBuildAutomation] APK gerado com SUCESSO! Tamanho: {summary.totalSize / (1024 * 1024):F2} MB em {summary.totalTime.TotalSeconds:F1}s.");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            else
            {
                Debug.LogError($"[AndroidBuildAutomation] Falha na compilação do APK: {summary.result} ({summary.totalErrors} erros).");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }
        }
    }
}
#endif
