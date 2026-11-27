# SYS_GAME_IDENTITY — Identidade Oficial e Metadados de RERY CITY

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.8 (Transformação em Jogo Independente)

---

## 1. Visão Geral e Identidade do Produto

O jogo passa a operar sob a identidade oficial definitiva **RERY CITY**, um simulador urbano independente de motocicletas, manobras e cultura do grau ambientado em uma representação futurista inspirada na cidade de Araguari, Minas Gerais.

A identidade anterior de desenvolvimento (*Projeto Grau 2*) fica restrita a namespaces internos de compilação C# (`ProjetoGrau2.*`) e histórico técnico, sendo totalmente dissociada da interface pública, telas de carregamento e menus do usuário final.

---

## 2. Metadados Centrais (`ReryCityIdentityConfig`)

A configuração é centralizada em um `ScriptableObject` editável:

```csharp
[CreateAssetMenu(fileName = "ReryCityIdentity_Default", menuName = "Rery City/Identity/Identity Config")]
public class ReryCityIdentityConfig : ScriptableObject
{
    private string gameTitle = "RERY CITY";
    private string publicSubtitle = "Simulador Urbano de Motocicletas e Manobras";
    private string versionString = "1.0.0-indie";
    private int buildNumber = 100;
    private string settingName = "Araguari Futurista";
    private bool forceOfflineMode = true;
    private bool allowSimulatedRewards = true;
    private bool enableTelemetry = false;
}
```

### Metadados e Diretrizes de Operação
- **Nome Oficial:** `RERY CITY`
- **Ambientação:** `Araguari Futurista`
- **Operação 100% Offline:** `forceOfflineMode = true` impede bloqueios ou dependência de servidores remotos.
- **Privacidade e Desacoplamento:** `enableTelemetry = false` garante zero envio de dados ou rastreamento invasivo.
- **Monetização Local:** `allowSimulatedRewards = true` autoriza mecânicas de recompensas locais sem requisições externas.

---

## 3. Rastreabilidade de Assets e GUIDs

- **Script:** `Assets/Scripts/Core/ReryCityIdentityConfig.cs` (`100000000000000000000000000000a6`)
- **Asset Padrão:** `Assets/ScriptableObjects/Config/ReryCityIdentity_Default.asset` (`200000000000000000000000000000a6`)
