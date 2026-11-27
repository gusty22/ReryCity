# SYS_WANTED_LEVEL — Sistema de Procurado e Nível de Alerta

---
type: system_specification
id: SYS-12.4
status: IMPLEMENTADO_EM_FASE_5_3
created: 2026-11-23
phase: 5
tags:
  - wanted_level
  - evasion
  - stars
  - heat_system
dependencies:
  - SYS_TRAFFIC_VIOLATIONS
  - SYS_POLICE_AI
---

## 1. Visão Geral

O componente `WantedLevelManager.cs` gerencia a notoriedade do jogador perante as autoridades, escalonando a severidade da resposta policial de 0 a 5 estrelas.

---

## 2. Escalonamento de Estrelas

* **Nível 0 (0 a 19 pts)**: Sem procurado. Patrulhas civis e policiais normais.
* **Nível 1 (20 a 59 pts)**: Infração identificada. Viaturas em campo ficam em estado `Suspicious`.
* **Nível 2 (60 a 129 pts)**: Acompanhamento policial. Ordem de parada por sirene.
* **Nível 3 (130 a 249 pts)**: Perseguição ativa. Giroflex, boost de motor e sirene contínua.
* **Nível 4 (250 a 449 pts)**: Reforços pesados. Entrada de viaturas ROCAM (motocicletas de alta agilidade).
* **Nível 5 (450+ pts)**: Cerco tático total e bloqueios viários.

---

## 3. Mecânica de Evasão e Esquecimento (Decay)

* Enquanto o jogador estiver fora do campo de visão de todas as viaturas (`isPlayerSpotted == false`), o cronômetro de evasão avança.
* A cada **15 segundos** contínuos fora do raio de visão policial, o nível de procurado decresce em 1 estrela, até retornar a zero.
* Caso qualquer viatura recupere contato visual, a contagem é instantaneamente zerada.
