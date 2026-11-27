# Sistema de População e Pedestres NPC (SYS_WORLD_POPULATION)

---
system_id: SYS_WORLD_POPULATION
parent_system: SYS-19 (Mundo Aberto)
status: IMPLEMENTADO_EM_FASE_5_7
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_WORLD]]
  - [[SYS_AI_VEHICLES]]
  - [[SYS_TRAFFIC_NAVIGATION]]
---

## 1. Visão Geral

O **Sistema de População e Pedestres** (`WorldPopulationManager`) controla a circulação de pedestres NPC nas calçadas e travessias urbanas do Projeto Grau 2, injetando vida orgânica à cidade com mínimo custo computacional.

---

## 2. Princípios de Operação

### 2.1. Object Pooling de Pedestres (`PedestrianPool`)
* Capacidade pré-alocada de 25 instâncias (`poolCapacity = 25`).
* Instâncias inativas aguardam em fila `Queue<PedestrianController>`, garantindo Zero GC no ciclo de spawn e despawn.

### 2.2. Comportamento e Reação a Perigo (`PedestrianController`)
* Estados: `Idle`, `Walking`, `CrossingStreet`, `Fleeing`.
* Quando uma motocicleta ou viatura em alta velocidade se aproxima a menos de $3.5\text{ m}$, o pedestre entra no estado `Fleeing`, acelerando para $3.6\text{ m/s}$ na direção de fuga para calçadas seguras.

### 2.3. Culling por Distância
* Pedestres a mais de $55\text{ m}$ do jogador são desativados e devolvidos ao pool imediatamente, mantendo a contagem ativa restrita a $15$ entidades no Android.

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `PedestrianController.cs` | `100000000000000000000000000000a1` | IA de locomoção em calçada e fuga |
| `PedestrianPool.cs` | `100000000000000000000000000000a2` | Pool pré-aquecido de instâncias NPC |
| `WorldPopulationManager.cs` | `100000000000000000000000000000a3` | Gerenciador de densidade e culling |
