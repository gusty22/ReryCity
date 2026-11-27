# Relatório de Pré-Auditoria Técnica — Fase 5.4
## Ocorrências Dinâmicas, Bloqueios Viários e Eventos do Mundo

---
documento: FASE_5_4_PRE_AUDIT.md
data: 2026-11-23
versao: 1.0.0
responsavel: AI Senior Software Engineer & Unity Systems Architect
status: APROVADO PARA IMPLEMENTACAO
---

## 1. Visão Geral da Missão

A **Fase 5.4** introduz dinamismo ambiental ao mundo urbano do **Projeto Grau 2**:
* O tráfego urbano deixa de ser estático e previsível, apresentando eventos emergentes: acidentes com veículos colididos, panes mecânicas no acostamento, obras na pista com desvios obrigatórios e cones de sinalização.
* O `WorldEventManager` gerencia a frequência, duração, ciclo de vida e espaçamento espacial de ocorrências ativas, prevenindo sobreposições.
* O `RoadBlockage` interdita trechos de faixas na `TrafficLane` e `TrafficWaypointNetwork`, forçando os veículos NPC e o jogador a reduzirem velocidade e utilizarem faixas adjacentes ou desvios.
* O `PoliceDispatchManager` integra ocorrências graves com a polícia (Fase 5.3), despachando viaturas em patrulha para atendimento no local do incidente.
* A infraestrutura é genérica e modular, pronta para receber futuramente o mapa e distritos da cidade de Araguari-MG sem refatorações em código.

---

## 2. Inventário de Dependências e Reutilização

| Sistema Existente | Papel e Reutilização na Fase 5.4 |
| :--- | :--- |
| `TrafficWaypointNetwork.cs` / `TrafficLane.cs` | Suporte a interdição de faixas (`isBlocked`), desvios viários e cálculo de rotas policiais até o local da ocorrência. |
| `AIVehicleController.cs` | Detecção de cones, veículos imobilizados e barreiras por `AIVehicleSensor` (`AvoidObstacle`, `Brake`, `LaneChanging`). |
| `AIVehiclePool.cs` | Fornece instâncias de veículos para simular carros avariados e acidentes sem alocação em tempo de execução. |
| `PolicePursuitController.cs` / `PoliceAIStateMachine.cs` | Viaturas policiais recebem despacho e navegam em estado de atendimento (`Investigating`) até a coordenada do sinistro. |
| `TrafficLODManager.cs` | Controle de LOD para ocorrências ativas: simplificação de eventos distantes e despawn ordenado. |

---

## 3. Arquitetura da Solução

```
[WorldEventManager] (Singleton Central, Controle de Limites e Spacing)
         |
         +---> [WorldEventConfig] (ScriptableObjects de Tipos de Ocorrência)
         |
         +---> [WorldEventInstance] (Estado, Raio, Temporizador, Cooldown)
         |           |
         |           +---> [RoadBlockage] (Cones, Barreiras, TrafficLane.isBlocked)
         |           +---> [NPC Vehicles Avariados / Acidentados] (do AIVehiclePool)
         |
         v
[PoliceDispatchManager] ---> Despacha viatura mais próxima (PolicePursuitController)
```

---

## 4. Matriz de GUIDs Padronizados da Fase 5.4

### 4.1. Scripts C# (`Assets/Scripts/World/`)
* `WorldEventType.cs`: `10000000000000000000000000000090`
* `WorldEventConfig.cs`: `10000000000000000000000000000091`
* `WorldEventInstance.cs`: `10000000000000000000000000000092`
* `RoadBlockage.cs`: `10000000000000000000000000000093`
* `WorldEventManager.cs`: `10000000000000000000000000000094`
* `PoliceDispatchManager.cs`: `10000000000000000000000000000095`

### 4.2. ScriptableObjects (`Assets/ScriptableObjects/WorldEvents/`)
* `EventConfig_TrafficAccident.asset`: `20000000000000000000000000000090`
* `EventConfig_RoadworkBlockage.asset`: `20000000000000000000000000000091`
* `EventConfig_BrokenVehicle.asset`: `20000000000000000000000000000092`
* `EventConfig_PoliceCheckpoint.asset`: `20000000000000000000000000000093`

---

## 5. Conclusão da Auditoria

Todas as interfaces e nós de integração com as Fases 5.1, 5.2 e 5.3 estão claros, estáveis e validados.  
**Auditoria técnica aprovada**: A implementação da Fase 5.4 pode ser iniciada imediatamente.
