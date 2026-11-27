# SYS_POLICE_AI — Inteligência Artificial Policial e Máquina de Estados

---
type: system_specification
id: SYS-12.1
status: IMPLEMENTADO_EM_FASE_5_3
created: 2026-11-23
phase: 5
tags:
  - police_ai
  - state_machine
  - rocam
  - pursuit_tactics
dependencies:
  - SYS_TRAFFIC_NAVIGATION
  - SYS_AI_VEHICLES
  - ADR_001_UNITY6_URP
  - ADR_002_CLEAN_ARCHITECTURE_PHYSICS
  - ADR_003_SCRIPTABLE_OBJECTS_DATA
---

## 1. Visão Geral

O subsistema **SYS_POLICE_AI** comanda a inteligência e as decisões das forças policiais no trânsito urbano do *Projeto Grau 2*.
Aplica regras táticas para viaturas convencionais (SUV 4x4 / Hilux) e motocicletas de patrulhamento tático (ROCAM - Triumph Tiger 800 / XT 660).

---

## 2. Máquina de Estados Finita (`PoliceAIStateMachine.cs`)

O comportamento policial é regido por 8 estados com histerese temporal (0.4s):

1. **Patrol**: Patrulhamento preventivo em comboio ou solitário, seguindo a velocidade da via e as faixas vetoriais.
2. **Suspicious**: Estado de atenção elevada após ruído acústico anormal ou passagem rápida de moto.
3. **Investigating**: Aproximação visual para identificação do veículo e checagem de infrações.
4. **Pursuit**: Perseguição ostensiva em alta velocidade com sirenes ativadas e aceleração com boost.
5. **Intercepting**: Manobra de corte de rota calculando a posição futura do infrator ($t_{ahead} = 1.8s$).
6. **Arresting**: Cerco e ordem de parada diante de infrator imobilizado ou cercado ($d \le 4.5m$, $v \le 8 km/h$).
7. **LostTarget**: Perda de contato visual direto; vasculha a Última Posição Conhecida (LKP) por até 12 segundos.
8. **ReturningToPatrol**: Desistência tática da perseguição e retorno ordenado ao fluxo viário civil.

---

## 3. Parametrização via ScriptableObjects

* `PoliceVehicleData.cs`: Massa, força motriz, boost de perseguição, capacidade de frenagem e inclinação (*lean*) para ROCAM.
* `PoliceAIData.cs`: Alcance visual (55m), ângulo do cone de visão (110°), raio auditivo (35m), tolerâncias de busca e tempos de abordagem.
