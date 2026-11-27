# Sistema de Ocorrências Dinâmicas e Eventos do Mundo (SYS_WORLD_EVENTS)

---
system_id: SYS_WORLD_EVENTS
parent_system: SYS-19 (Mundo Aberto)
status: IMPLEMENTADO_EM_FASE_5_4
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_TRAFFIC_NAVIGATION]]
  - [[SYS_ROAD_BLOCKAGE]]
  - [[SYS_POLICE_DISPATCH]]
  - [[SYS_AI_VEHICLES]]
---

## 1. Visão Geral

O **Sistema de Ocorrências Dinâmicas e Eventos do Mundo** (`WorldEventManager`) é o subsistema responsável por instanciar, monitorar e concluir eventos incidentais nas vias urbanas do Projeto Grau 2, injetando imprevisibilidade e dinamismo orgânico ao mapa da cidade sem impactar a estabilidade do fluxo de tráfego.

Dentre as tipologias de eventos suportadas estão:
1. **Acidentes de Trânsito (`TrafficAccident`)**: Colisão entre veículos civis com bloqueio parcial ou total de faixa e solicitação obrigatória de apoio policial.
2. **Obras Viárias (`RoadworkBlockage`)**: Interdições programadas com sinalização tática (cones e barreiras) e redirecionamento de faixa.
3. **Pane Mecânica (`BrokenVehicle`)**: Veículos enguiçados na pista com pisca-alerta ativo.
4. **Comando/Blitz Policial (`PoliceCheckpoint`)**: Bloqueio de fiscalização com viaturas ostensivas e funil de trânsito.

---

## 2. Arquitetura e Componentes C#

### 2.1. `WorldEventType`
Enum fortemente tipado definindo as categorias de ocorrências:
* `None` (0)
* `TrafficAccident` (1)
* `RoadworkBlockage` (2)
* `BrokenVehicle` (3)
* `PoliceCheckpoint` (4)
* `ProtestOrParade` (5)
* `Custom` (99)

### 2.2. `WorldEventConfig` (ScriptableObject)
Define parâmetros de spawn, regras e restrições:
* `eventType`: Tipo da ocorrência.
* `minDuration` / `maxDuration`: Tempo de permanência no mundo.
* `blocksTrafficLane`: Sinaliza se a faixa onde nasceu deve ser interditada.
* `requiresPoliceDispatch`: Se despacha viaturas disponíveis via `PoliceDispatchManager`.
* `spawnWeight`: Probabilidade relativa de sorteio.
* `propPrefabs`: Prefabs a instanciar (cones, triângulos, veículos avariados).

### 2.3. `WorldEventInstance` (MonoBehaviour)
Instância viva de uma ocorrência no mundo com máquina de estados:
* Estados: `Pending` -> `Active` -> `Resolving` -> `Completed` / `Cancelled`.
* Atributos: `AssociatedLane`, `AssociatedBlockage`, `RemainingTime`.
* Transição de encerramento suave com remoção limpa de adereços sem *garbage collection*.

### 2.4. `WorldEventManager` (MonoBehaviour / Singleton)
Orquestrador central do ciclo de vida dos eventos:
* Garante espaçamento espacial anti-sobreposição (`minSpawnDistance = 35m`, `maxSpawnDistance = 90m` do jogador).
* Controle estrito de teto de ocorrências simultâneas (`maxActiveEvents = 3`).
* Intervalo de verificação temporizado (`eventSpawnInterval = 25s`).
* Acoplamento desacoplado via eventos C# (`OnEventStarted`, `OnEventFinished`).

---

## 3. Otimizações Mobile e Perfil Zero GC

* **Arrays estáticos e Pools**: Props e obstáculos reutilizam pools de objetos já existentes.
* **Sem alocações em Update**: Timers internos operam sobre listas de tamanho fixo sem criação de `new List` ou `Linq`.
* **Despawn por Proximidade**: Eventos muito distantes do jogador (> 120m) entram em encerramento assíncrono para liberar memória.

---

## 4. Matriz de Rastreabilidade

| Script | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `WorldEventType.cs` | `10000000000000000000000000000090` | Enumeração de tipos de ocorrências |
| `WorldEventConfig.cs` | `10000000000000000000000000000091` | Configuração orientada a dados ScriptableObject |
| `WorldEventInstance.cs` | `10000000000000000000000000000092` | Ciclo de vida da instância ativa |
| `WorldEventManager.cs` | `10000000000000000000000000000094` | Gerenciador global de spawn e ciclo de eventos |
