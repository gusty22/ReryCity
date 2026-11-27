# Relatório de Pré-Auditoria Técnica — Fase 5.8: RERY CITY

**Data:** 23/11/2026  
**Status:** Aprovado para Implementação  
**Escopo:** Transição definitiva para identidade autônoma e independente (*RERY CITY*), eliminação de dependências de anúncios externos, inicialização desacoplada 100% offline, carregamento assíncrono real e perfis de gerenciamento de energia mobile.

---

## 1. Diagnóstico do Estado Atual

### 1.1 Análise de Dependências Externas e SDKs de Anúncios
- **Varredura em `Packages/manifest.json`**:
  - Não há pacotes externos de anúncios (Google Mobile Ads, IronSource, Unity Ads, AppLovin).
  - Apenas pacotes padrão Unity URP, Input System, Cinemachine, TextMeshPro e módulos nativos.
- **Varredura no Código C# (`Assets/Scripts/`)**:
  - Zero referências a bibliotecas de AdMob ou telemetria invasiva de terceiros.
- **Diretriz de Monetização/Recompensas**:
  - Conforme solicitado, deve ser implementado um botão e fluxo de **Recompensa Simulada** ("Assistir anúncio para ganhar recompensa") que opera **100% offline**, com temporizador de simulação local (cooldown/debouncing de segurança, persistência em disco via SaveSystem e retorno imediato de itens/dinheiro no jogo), sem qualquer requisição de rede ou SDK externo.

### 1.2 Identidade Pública vs. Identidade Interna
- **Identidade Pública Oficial**:
  - Nome do Jogo: **RERY CITY**
  - Ambientação: Representação urbana inspirada em Araguari, MG, em cenário futurista.
  - Subtítulo / Descrição: Simulador urbano independente de motocicletas e manobras.
  - Versão: 1.0.0-indie.
- **Identidade Interna e Código**:
  - O namespace `ProjetoGrau2.*` e estrutura interna de projeto permanecem para estabilidade de compilação, GUIDs e histórico técnico.
  - A identidade pública passa a ser centralizada em `ReryCityIdentityConfig` (ScriptableObject).

### 1.3 Fluxo de Inicialização e Carregamento Assíncrono
- **Fluxo Offline Direto**:
  - Abertura $\rightarrow$ Verificação de estado local $\rightarrow$ Carregamento do Menu Principal $\rightarrow$ Transição assíncrona para Gameplay.
  - Zero bloqueios por checagem de internet, zero popups intersticiais intrusivos.
- **Carregador Assíncrono (`AsynchronousSceneLoader`)**:
  - Monitoramento contínuo de progresso real (`AsyncOperation.progress`), interpolação suave para HUD e disparo de eventos de ciclo de vida (`OnLoadStarted`, `OnProgressUpdated`, `OnLoadCompleted`).

### 1.4 Perfis de Energia e Desempenho Mobile
- Criação do `MobileEnergyProfileManager` com 3 perfis configuráveis:
  1. `Economico`: 30 FPS, LOD reduzido, sombras otimizadas, economia de bateria e gerenciamento térmico agressivo.
  2. `Equilibrado`: 45/60 FPS, equilíbrio dinâmico entre fluidez e consumo de bateria.
  3. `AltoDesempenho`: 60 FPS incondicional, resolução e qualidade gráfica máximas.

---

## 2. Alocação Determinística de GUIDs

| Componente / Arquivo | Tipo | GUID Determinístico |
| :--- | :--- | :--- |
| `ReryCityIdentityConfig.cs` | Script | `100000000000000000000000000000a6` |
| `OfflineGameBootstrapper.cs` | Script | `100000000000000000000000000000a7` |
| `SimulatedRewardManager.cs` | Script | `100000000000000000000000000000a8` |
| `AsynchronousSceneLoader.cs` | Script | `100000000000000000000000000000a9` |
| `MobileEnergyProfileManager.cs` | Script | `100000000000000000000000000000aa` |
| `ReryCityIdentity_Default.asset` | ScriptableObject Asset | `200000000000000000000000000000a6` |
| `MobileEnergyProfile_Default.asset` | ScriptableObject Asset | `200000000000000000000000000000a7` |

---

## 3. Plano de Execução

1. Criar pasta `Assets/Scripts/Identity/` ou `Assets/Scripts/Core/` e implementar `ReryCityIdentityConfig.cs` e `OfflineGameBootstrapper.cs`.
2. Implementar `SimulatedRewardManager.cs` em `Assets/Scripts/Monetization/`.
3. Implementar `AsynchronousSceneLoader.cs` em `Assets/Scripts/Core/`.
4. Implementar `MobileEnergyProfileManager.cs` em `Assets/Scripts/Performance/`.
5. Criar os assets ScriptableObject correspondentes com GUIDs e meta files determinísticos.
6. Integrar os componentes ao GameObject central `ReryCity_GameManager` na cena `UrbanTrafficTestScene.unity`.
7. Criar e executar a suíte de testes `test_rery_city_offline.py` e regressão completa das Fases 5.1 a 5.8.
8. Atualizar Obsidian Vault, Graphify Knowledge Graph e emitir documentação oficial de conclusão.
