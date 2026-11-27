# SYS_ENERGY_MANAGEMENT — Gerenciamento de Energia e Perfis Mobile

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.8 (Transformação em Jogo Independente)

---

## 1. Visão Geral

O gerenciamento de energia de **RERY CITY** implementa 3 perfis configuráveis de desempenho (`MobileEnergyProfileManager`), permitindo ao jogador adaptar o consumo de bateria, aquecimento do dispositivo e fidelidade visual.

---

## 2. Perfis de Energia e Configurações

| Parâmetro | Econômico | Equilibrado (Padrão) | Alto Desempenho |
| :--- | :--- | :--- | :--- |
| **FPS Alvo** (`Application.targetFrameRate`) | **30 FPS** | **60 FPS** | **60 FPS** |
| **LOD Bias** (`QualitySettings.lodBias`) | `0.7` | `1.0` | `1.5` |
| **Sombras** (`QualitySettings.shadows`) | Desativadas (`Disable`) | Duras (`HardOnly`) | Suaves/Completas (`All`) |
| **Distância de Sombras** (`shadowDistance`) | `25m` | `50m` | `80m` |
| **Prevenção de Sono** (`Screen.sleepTimeout`) | Padrão do Sistema | `NeverSleep` | `NeverSleep` |
| **Foco Operacional** | Bateria & Térmico | Equilíbrio Fluidez | Fidelidade Máxima |

---

## 3. Persistência e Alternância em Tempo Real

- A seleção do perfil é persistida localmente via `PlayerPrefs` (`ReryCity_EnergyProfile`).
- Alternância imediata sem necessidade de reiniciar a aplicação através de `SetProfile(EnergyProfileMode mode)`.
- Evento C# desacoplado `OnProfileChanged(EnergyProfileMode, EnergyProfileSettings)` emitido para componentes visuais e de telemetria.

---

## 4. Rastreabilidade de Assets e GUIDs

- **Script:** `Assets/Scripts/Performance/MobileEnergyProfileManager.cs` (`100000000000000000000000000000aa`)
- **Script Config:** `Assets/Scripts/Performance/MobileEnergyProfileConfig.cs` (`100000000000000000000000000000ab`)
- **Asset Padrão:** `Assets/ScriptableObjects/Config/MobileEnergyProfile_Default.asset` (`200000000000000000000000000000a7`)
- **GameObject na Cena:** Integrado em `ReryCity_GameManager` (`fileID: 1185` em `UrbanTrafficTestScene.unity`).
