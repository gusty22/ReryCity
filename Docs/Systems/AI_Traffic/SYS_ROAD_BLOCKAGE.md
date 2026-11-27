# Sistema de Bloqueios Viários e Desvios Táticos (SYS_ROAD_BLOCKAGE)

---
system_id: SYS_ROAD_BLOCKAGE
parent_system: SYS-13 (Tráfego Urbano)
status: IMPLEMENTADO_EM_FASE_5_4
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_TRAFFIC_NAVIGATION]]
  - [[SYS_AI_VEHICLES]]
  - [[SYS_WORLD_EVENTS]]
---

## 1. Visão Geral

O **Sistema de Bloqueios Viários** (`RoadBlockage`) gerencia a interdição física e lógica de trechos de tráfego urbano em decorrência de obras, acidentes ou comandos policiais.

Em vez de reconstruir grafos globais estáticos em tempo de execução, o sistema atua dinamicamente na propriedade `TrafficLane.isBlocked`. Os controladores autônomos (`AIVehicleController`) e sensores de obstáculos (`AIVehicleSensor`) identificam o bloqueio da faixa adiante e iniciam manobras táticas de mudança de faixa (`InitiateLaneChange`) para vias desimpedidas ou realizam paradas suaves sem colisões em cadeia.

---

## 2. Princípios de Funcionamento

### 2.1. Interdição Direta de Faixa (`TrafficLane.isBlocked`)
* Quando `RoadBlockage.ApplyBlockage()` é acionado, `blockedLane.isBlocked` torna-se `true`.
* Veículos que planejam acessar essa faixa rejeitam o trajeto em suas avaliações de nós e conexões de cruzamentos.
* Veículos já inseridos na faixa ativam rotina de desvio imediato ou desaceleração progressiva.

### 2.2. Sinalização Tática com Cones e Barreiras
* Cones de trânsito (`Obstacle_Roadwork_Cone`) e cavaletes são distribuídos linearmente ao longo de `blockageLength` (10m a 60m).
* Quantidade configurável de cones (2 a 12), posicionados com espaçamento proporcional à curvatura da pista.
* Obstáculos possuem colliders físicos ativos, permitindo colisão com a motocicleta do jogador e provocando desequilíbrio se abalroados em alta velocidade.

### 2.3. Desvio e Re-roteamento Autônomo de NPCs
* `AIVehicleController` avalia `currentLane.isBlocked`.
* Se houver `parallel_lane` não bloqueada:
  - Aciona indicador lateral (pisca-alerta/seta).
  - Executa transição suave via `InitiateLaneChange` com amortecimento lateral.
* Se não houver faixa alternativa:
  - Aplica desaceleração proporcional até a parada segura a 5m do obstáculo frontal, evitando colisões traseiras em série.

### 2.4. Liberação e Limpeza (`ClearBlockage()`)
* Quando o evento associado encerra ou expira (`WorldEventState.Completed`):
  - `blockedLane.isBlocked` é redefinido para `false`.
  - Cones e elementos cênicos são devolvidos ao pool ou destruídos ordenadamente.
  - O fluxo natural de veículos é restabelecido instantaneamente.

---

## 3. Matriz de Componentes

| Componente | GUID | Função Principal |
| :--- | :--- | :--- |
| `RoadBlockage.cs` | `10000000000000000000000000000093` | Controlador de interdição e geração de obstáculos |
| `TrafficLane.cs` | `10000000000000000000000000000041` | Contém propriedade `isBlocked` (flag lógica de navegação) |
| `AIVehicleController.cs` | `10000000000000000000000000000057` | Lógica de evasão, parada e mudança de faixa por bloqueio |
