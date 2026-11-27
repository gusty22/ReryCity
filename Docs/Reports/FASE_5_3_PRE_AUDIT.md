# Relatório de Pré-Auditoria Técnica — Etapa 5.3
## Inteligência Artificial Policial e Sistema de Procurado

---
documento: FASE_5_3_PRE_AUDIT.md
data: 2026-11-23
versao: 1.0.0
responsavel: AI Senior Software Engineer & Unity Systems Architect
status: APROVADO PARA IMPLEMENTACAO
---

## 1. Visão Geral da Missão

A **Etapa 5.3** introduz o sistema de aplicação da lei e autoridade no trânsito urbano do **Projeto Grau 2**:
* IA de Viaturas Convencionais (SUV/Hilux) e Motocicletas ROCAM (Tiger 800 / XT 660).
* Máquina de 8 estados policiais (`Patrol`, `Suspicious`, `Investigating`, `Pursuit`, `Intercepting`, `Arresting`, `LostTarget`, `ReturningToPatrol`).
* Sistema de Detecção de Infrações de Trânsito (`TrafficViolationDetector`): excesso de velocidade, contramão, empinar moto (grau) em via pública, avanço de semáforo vermelho e fuga de ordem de parada.
* Gerenciador de Nível de Procurado (`WantedLevelManager`): progressão e decay de 0 a 5 estrelas.
* Controlador de Perseguição e Interceptação (`PolicePursuitController`): navegação pela `TrafficWaypointNetwork`, cálculo de perda de contato visual e busca da última posição conhecida (*Last Known Position*).
* Interface de apresentação: `PoliceHUD` e `WantedLevelUI`.

---

## 2. Inventário e Capacidade de Reutilização dos Componentes

| Componente Base | Status | Papel na Etapa 5.3 |
| :--- | :--- | :--- |
| `TrafficWaypointNetwork.cs` | Operacional (5.1) | Usado pela IA policial para roteamento, busca de rotas e cálculo de intersecções à frente. |
| `TrafficLane.cs` | Operacional (5.1) | Verificação de contramão (`direction` vs `player.forward`) e limite regulamentado de velocidade (`speedLimitKmh`). |
| `TrafficSignal.cs` | Operacional (5.1) | Detecção de avanço em sinal vermelho (`IsRed`). |
| `AIVehicleMotorController.cs` | Operacional (5.2) | Propulsão longitudinal física via `Rigidbody.AddForce` no veículo policial. |
| `AIVehicleSteeringController.cs` | Operacional (5.2) | Esterçamento e inclinação lateral procedural (*lean banking*) para motos da ROCAM. |
| `AIVehicleBrakeController.cs` | Operacional (5.2) | Frenagem controlada e parada de abordagem. |
| `AIVehicleSensor.cs` | Operacional (5.2) | Percepção de obstáculos civis para evitar colisões durante perseguições. |
| `WheeliePhysicsController.cs` | Operacional (1.0) | Fornece `CurrentPitchAngle` (> 25°) para detecção de manobra perigosa (grau). |
| `MotorcycleController.cs` | Operacional (1.0) | Fornece velocidade real da moto do jogador (`currentSpeedKmh`). |
| `AIVehiclePool.cs` / `TrafficLODManager.cs` | Operacional (5.2) | Pooling de viaturas policiais e controle de taxa de sensores por distância. |

---

## 3. Arquitetura da Solução

```
[Player Motorcycle] (Speed, Direction, Pitch)
         |
         v
[TrafficViolationDetector] ---> Emite Infrações com Cooldown (Zero GC)
         |
         v
[WantedLevelManager] (Níveis 0 a 5, Timer de Evasão/Decay)
         |
         v
[PolicePursuitController] <---> [PoliceAIStateMachine] (8 Estados)
         |                                 |
         +--> [PoliceVehicleData]          +--> [TrafficNavigationManager]
         +--> [PoliceAIData]               +--> [AIVehicleMotorController]
         |                                 +--> [AIVehicleSteeringController]
         v                                 +--> [AIVehicleBrakeController]
[PoliceHUD / WantedLevelUI] (Estrelas, Sirene, Distância, Alertas)
```

---

## 4. Matriz de GUIDs Determinísticos Padronizados

### 4.1. Scripts C# (`Assets/Scripts/Police/` & `Assets/Scripts/UI/`)
* `PoliceVehicleData.cs`: `10000000000000000000000000000071`
* `PoliceAIData.cs`: `10000000000000000000000000000072`
* `PoliceAIStateMachine.cs`: `10000000000000000000000000000073`
* `TrafficViolationDetector.cs`: `10000000000000000000000000000074`
* `WantedLevelManager.cs`: `10000000000000000000000000000075`
* `PolicePursuitController.cs`: `10000000000000000000000000000076`
* `PoliceHUD.cs`: `10000000000000000000000000000077`
* `WantedLevelUI.cs`: `10000000000000000000000000000078`

### 4.2. ScriptableObjects (`Assets/ScriptableObjects/Police/`)
* `PoliceVehicleData_ViaturaHilux.asset`: `20000000000000000000000000000080`
* `PoliceVehicleData_RocamTiger800.asset`: `20000000000000000000000000000081`
* `PoliceAIData_StandardPatrol.asset`: `20000000000000000000000000000082`
* `PoliceAIData_RocamPursuit.asset`: `20000000000000000000000000000083`

---

## 5. Conclusão da Auditoria

Nenhuma incompatibilidade encontrada. O sistema reaproveita 100% da infraestrutura física e de navegação existente.  
**Auditoria aprovada**: A implementação da Etapa 5.3 pode prosseguir.
