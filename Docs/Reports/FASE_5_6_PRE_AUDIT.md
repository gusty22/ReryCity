# Relatório de Pré-Auditoria Técnica — Fase 5.6: Integração Geral, HUD e Experiência do Jogador

---
auditoria_id: FASE_5_6_PRE_AUDIT
data: 2026-11-23
projeto: Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)
status: APROVADO_PARA_EXECUCAO
autor: AI Senior Software Engineer & Unity Systems Architect
---

## 1. Contexto e Objetivo da Fase 5.6

A **Fase 5.6** consolida todas as etapas anteriores (Fases 0 a 4 do núcleo físico da moto e Fases 5.1 a 5.5 da malha de tráfego, veículos NPC, IA policial e ocorrências do mundo) em uma **camada de apresentação e experiência do usuário (HUD e Controles Mobile)** coesa, desacoplada e otimizada para dispositivos Android e PC.

---

## 2. Inventário de Sistemas Existentes e Dependências

### 2.1. Fontes de Dados e Controle Físico Real (Sem Valores Fictícios)
* `MotorcycleController.cs` (`GUID: 1000...02`): Velocidade real em km/h (`CurrentSpeedKmh`), contato com solo (`IsFrontGrounded`, `IsRearGrounded`).
* `EngineController.cs` (`GUID: 1000...03`): RPM (`CurrentRpm`), corte de giro (`IsCutActive`), aceleração efetiva (`EffectiveThrottle`), torque e potência reais.
* `TransmissionController.cs` (`GUID: 1000...04`): Marcha engatada (`CurrentGear`), embreagem (`ClutchEngagement`), estado de troca (`IsShifting`).
* `AthlonECUController.cs` (`GUID: 1000...05`): Estado de Two-Step (`IsTwoStepActive`), corte programável (`IsCutting`), disparo de estampidos (`OnBackfireTriggered`).
* `WheeliePhysicsController.cs` (`GUID: 1000...01`): Ângulo de inclinação do grau (`CurrentPitchAngle`), cronômetro de empinada (`TimeInWheelie`), estado de queda (`IsCrashed`).
* `MotorcycleCameraController.cs` (`GUID: 1000...06`): Modo de visão atual (`CurrentMode`: `ThirdPersonClose`, `ThirdPersonFar`, `FirstPerson`, `Hood`, `DynamicWheelie`).
* `CrossPlatformInputHandler.cs` (`GUID: 1000...10`): Interface `IMotorcycleInput` com métodos de entrada virtual já preparados (`SetVirtualThrottle`, `SetVirtualSteering`, `SetVirtualWheelie`, `SetVirtualRearBrake`, etc.).

### 2.2. Sistemas de Polícia e Ocorrências Existentes
* `WantedLevelManager.cs` (`GUID: 1000...75`): Nível de estrelas (0 a 5) e progresso de evasão.
* `TrafficViolationDetector.cs` (`GUID: 1000...74`): Eventos de infrações (`OnViolationCommitted`).
* `PolicePursuitCoordinator.cs` (`GUID: 1000...97`): Estado de perseguição ativa, avistamento de suspeito, LKP e viaturas engajadas.
* `WorldEventManager.cs` (`GUID: 1000...94`): Eventos `OnEventStarted` e `OnEventFinished` com distância espacial e tipo de ocorrência.
* `PoliceHUD.cs` (`GUID: 1000...77`) e `WantedLevelUI.cs` (`GUID: 1000...78`): Componentes visuais para interface policial.

### 2.3. Áudio e Smartphone Virtual
* `MotorAudioController.cs` (`GUID: 1000...08`): Controlador dinâmico com fontes de áudio de motor, trocas de marcha e estampidos.
* `AthlonMobileApp.cs` (`GUID: 1000...09`): Painel de celular virtual com perfis de injeção e Two-Step.

---

## 3. Matriz de Componentes Novos e GUIDs Determinísticos

| Componente | Tipo | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- | :--- |
| `MotorcycleHUD.cs` | Script (MonoBehaviour) | `10000000000000000000000000000098` | Painel de instrumentos principal: Velocímetro, Tacômetro/RPM, Marcha, Corte de Giro, Two-Step, Ângulo de Grau, Câmera e Alertas mecânicos com Zero GC |
| `WorldEventNotificationUI.cs` | Script (MonoBehaviour) | `10000000000000000000000000000099` | Feed de avisos contextuais de ocorrências viárias (Acidentes, Obras, Bloqueios, Blitz) com distância ao jogador e priorização |
| `TouchscreenMobileControlsUI.cs` | Script (MonoBehaviour) | `1000000000000000000000000000009A` | Controles táteis mobile para Android: acelerador, freios dianteiro/traseiro, manete de grau, trocas de marcha, alternância de câmera e botão do app Athlon |
| `AudioIntegrationManager.cs` | Script (MonoBehaviour) | `1000000000000000000000000000009B` | Roteador e gerenciador de canais de áudio: motor, escapamento, sirenes policiais espaciais e efeitos sonoros de interface |
| `UrbanTrafficTestScene.unity` | Cena (Modificada) | N/A | Atualização do Canvas com suporte a múltiplas resoluções e Safe Areas |

---

## 4. Diretrizes de Otimização para Android (Zero GC)

1. **Atualização Condicional e Dirty Flags**:
   - Textos de velocidade e RPM não executam `ToString()` e concatenação de strings a cada frame em `Update()`. Apenas atualizam quando o valor inteiro ou estado mudar, ou a cada intervalo calibrado (ex: $0.05\text{ s}$ / 20 Hz).
2. **Canvas Scaler e Safe Area**:
   - `CanvasScaler` em modo `ScaleWithScreenSize`, resolução de referência $1920 \times 1080$, `matchWidthOrHeight = 0.5`.
   - Âncoras flexíveis para evitar sobreposição em entalhes de câmera (Notch) e cantos arredondados de telas Android.
3. **Desacoplamento por Eventos C#**:
   - Mudanças de marcha, infrações e eventos do mundo acionam atualizações de UI estritamente reativas via delegates/events.
4. **Isolamento de Áudio**:
   - Uso de `AudioSource` pré-alocados em componentes dedicados, evitando `PlayClipAtPoint` que instancia e destrói GameObjects em tempo de execução.

---

## 5. Plano de Testes Automatizados (`test_ui_integration.py`)

A nova suíte verificará formalmente:
1. Integridade de scripts e metadados GUID determinísticos da Fase 5.6.
2. Presença de `MotorcycleHUD`, `WorldEventNotificationUI`, `TouchscreenMobileControlsUI` e `AudioIntegrationManager` na cena `UrbanTrafficTestScene.unity`.
3. Extração fidedigna de telemetria da moto (Velocidade km/h, RPM real, Marcha, Corte de Giro, Two-Step, Grau).
4. Integração da interface de infrações e perseguição policial.
5. Notificações contextuais de eventos do mundo com cálculo de proximidade.
6. Mapeamento dos botões táteis virtuais para `CrossPlatformInputHandler`.
7. Conexão do aplicativo virtual Athlon e áudio desacoplado.
8. Regressão estrita de 100% das 5 suítes anteriores (5.1 a 5.5).
