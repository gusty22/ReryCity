# Sistema de Despacho Policial e Atendimento a Sinistros (SYS_POLICE_DISPATCH)

---
system_id: SYS_POLICE_DISPATCH
parent_system: SYS-12 (Polícia e Perseguição)
status: IMPLEMENTADO_EM_FASE_5_4
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_POLICE_AI]]
  - [[SYS_POLICE_PURSUIT]]
  - [[SYS_WORLD_EVENTS]]
  - [[SYS_AI_VEHICLES]]
---

## 1. Visão Geral

O **Sistema de Despacho Policial** (`PoliceDispatchManager`) atua como a central de comunicações táticas (COPOM) do Projeto Grau 2, integrando a frota de patrulha ostensiva às ocorrências dinâmicas que emergem na cidade.

Quando eventos do tipo `TrafficAccident` ou `PoliceCheckpoint` são gerados, o despachador enfileira a ocorrência e localiza a viatura ou moto ROCAM mais próxima que esteja em patrulhamento preventivo, ordenando deslocamento em código de emergência com sirenes e giroflex ativados.

---

## 2. Princípios de Operação

### 2.1. Fila de Ocorrências (`_pendingIncidents`)
* Ocorrências que demandam presença policial chamam `DispatchIncident(incident)`.
* As ocorrências aguardam na fila FIFO até que uma viatura elegível esteja livre.

### 2.2. Algoritmo de Seleção da Viatura Mais Próxima
* Critérios de elegibilidade:
  - Viatura deve estar ativa e no estado `Patrol` ou `ReturningToPatrol`.
  - Não pode estar engajada em perseguição ativa ao jogador (`Pursuit`, `Intercepting`, `Arresting`).
  - Não pode estar previamente atribuída a outro sinistro.
* Avalia a menor distância euclidiana ao quadrado (`sqrMagnitude`) para evitar cálculo desnecessário de raiz quadrada no loop.

### 2.3. Resposta Tática e Deslocamento
* Ao receber o chamado:
  - O estado da viatura é alterado para `PoliceAIState.Investigating`.
  - Sirene e giroflex são ligados (`isSirenActive = true`).
  - O alvo de navegação do `AIVehicleSteeringController` é setado diretamente nas coordenadas da ocorrência (`incident.Position`).

### 2.4. Chegada, Atendimento e Retorno à Patrulha
* Ao aproximar-se a menos de `12m` do ponto do evento:
  - Dispara o evento `OnIncidentArrived`.
  - A ocorrência entra na fase `WorldEventState.Resolving`.
* Após a conclusão ou expiração do evento (`Completed`):
  - A viatura é desvinculada do sinistro.
  - Sirenes são desligadas.
  - O estado é alterado para `ReturningToPatrol`, permitindo retomar o roteamento de patrulha regular ou aceitar novos chamados.

---

## 3. Matriz de Componentes

| Componente | GUID | Responsabilidade |
| :--- | :--- | :--- |
| `PoliceDispatchManager.cs` | `10000000000000000000000000000095` | Despacho centralizado de ocorrências para viaturas |
| `PolicePursuitController.cs` | `10000000000000000000000000000076` | Controlador veicular e tático da viatura policial |
| `PoliceAIStateMachine.cs` | `10000000000000000000000000000073` | Máquina de estados policiais (inclui `Investigating`) |
| `WorldEventInstance.cs` | `10000000000000000000000000000092` | Entidade do sinistro que solicita apoio |
