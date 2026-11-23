using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Persistence
{
    [Serializable]
    public class PlayerProgressData
    {
        public string selectedMotorcycleId = "moto_titan_160";
        public List<string> ownedMotorcycleIds = new List<string> { "moto_titan_160" };
        public List<string> installedPartIds = new List<string>();
        public int playerMoneyCentavos = 150000; // R$ 1.500,00 inicial
        public int activeCameraModeIndex = 0;

        // Configuração salva da Athlon
        public float savedMainCutRpm = 10200f;
        public float savedTwoStepRpm = 5200f;
        public bool savedTwoStepActive = true;
        public int savedCutMode = 0; // 0 = IgnitionCut
    }

    /// <summary>
    /// Sistema de persistência e salvamento multiplataforma (PC e Android).
    /// Serializa dados de motocicletas, peças da oficina, economia e mapas Athlon em JSON.
    /// Salva em Application.persistentDataPath, imune a atualizações de APK ou builds.
    /// </summary>
    public static class SaveSystem
    {
        private static readonly string SaveFileName = "savegame_progression.json";

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(PlayerProgressData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Falha ao salvar dados do jogador: {ex.Message}");
            }
        }

        public static PlayerProgressData Load()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    return JsonUtility.FromJson<PlayerProgressData>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveSystem] Falha ao carregar savegame: {ex.Message}. Criando perfil padrão.");
            }

            return new PlayerProgressData();
        }

        public static void DeleteSave()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
        }
    }
}
