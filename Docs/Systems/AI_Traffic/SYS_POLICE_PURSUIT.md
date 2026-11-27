# SYS_POLICE_PURSUIT — Perseguição Policial e Táticas de Interceptação

---
type: system_specification
id: SYS-12.2
status: IMPLEMENTADO_EM_FASE_5_3
created: 2026-11-23
phase: 5
tags:
  - police_pursuit
  - interception
  - line_of_sight
  - last_known_position
dependencies:
  - SYS_POLICE_AI
  - SYS_TRAFFIC_NAVIGATION
  - SYS_WANTED_LEVEL
---

## 1. Visão Geral

O componente `PolicePursuitController.cs` orquestra o comportamento físico de perseguição da guarnição, integrando o rastreamento da moto do jogador à malha viária vetorial.

---

## 2. Táticas Operacionais

### 2.1. Percepção e Cone de Visão
* Raycast não-alocativo (`Physics.RaycastNonAlloc`) de linha de visada direta entre os olhos da viatura e o chassi da moto.
* Ângulo de abertura do cone de visão calibrado em 110°.
* Obstáculos como edifícios, caminhões e muros interrompem o contato visual, forçando a viatura a salvar a **Última Posição Conhecida (Last Known Position - LKP)**.

### 2.2. Interceptação Dinâmica
Ao ingressar no estado `Intercepting`, a viatura calcula o ponto de encontro futuro com base na velocidade longitudinal da moto:
$$\vec{P}_{intercept} = \vec{P}_{player} + \vec{v}_{player} \cdot t_{ahead}$$
onde $t_{ahead} \in [1.4s, 1.8s]$.

### 2.3. Abordagem e Prisão (Busted)
* Condições: Distância $\le 4.5m$ e velocidade do jogador $\le 8 km/h$.
* Se o jogador permanecer contido por 3.0 segundos ininterruptos, o evento `OnPlayerBusted` é emitido, desativando a perseguição e limpando o nível de procurado.
* Se o jogador arrancar bruscamente, o estado é cancelado e uma infração de desobediência (`EvadingPolice`) é autuada.
