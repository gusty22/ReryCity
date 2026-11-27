# Sistema de Coordenação Avançada de Perseguições Policiais (SYS_PURSUIT_COORDINATION)

---
system_id: SYS_PURSUIT_COORDINATION
parent_system: SYS-12 (Polícia e Perseguição)
status: IMPLEMENTADO_EM_FASE_5_5
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_POLICE_AI]]
  - [[SYS_POLICE_PURSUIT]]
  - [[SYS_POLICE_DISPATCH]]
  - [[SYS_WANTED_LEVEL]]
  - [[SYS_TRAFFIC_NAVIGATION]]
---

## 1. Visão Geral

O **Sistema de Coordenação Avançada de Perseguições Policiais** (`PolicePursuitCoordinator`) gerencia operações conjuntas e táticas entre múltiplas viaturas policiais (Cruisers 4x4) e motocicletas de patrulhamento tático (ROCAM), simulando os procedimentos de cerco e acompanhamento da Polícia Militar brasileira (ROCAM / Força Tática / COPOM).

O sistema evita comportamentos artificiais como aglomeração de viaturas no mesmo ponto, retransmite em tempo real a Última Posição Conhecida (LKP) do suspeito via rádio, distribui papéis táticos especializados e organiza buscas por quadrantes quando o contato visual é interrompido.

---

## 2. Arquitetura e Papéis Táticos (`PoliceTacticalRole`)

### 2.1. Especialização por Tipo de Veículo
* **`LeadPursuer` (Viatura Líder)**:
  - Prioridade atribuída a motocicletas **ROCAM** (Triumph Tiger 800) devido à relação peso/potência e agilidade entre o tráfego urbano e corredores estreitos.
  - Persegue diretamente na esteira da motocicleta do jogador a uma distância de segurança ($8\text{ m}$ a $15\text{ m}$).
* **`FlankerLeft` e `FlankerRight` (Flanqueadores)**:
  - Viaturas pesadas 4x4 (Hilux / Trailblazer) que se posicionam lateralmente e recuadas ($4\text{ m}$ de offset lateral, $6\text{ m}$ atrás do alvo).
  - Fecham corredores de escape e impedem desvios laterais ou retornos repentinos.
* **`Interceptor` (Interceptador Frontal)**:
  - Viatura pesada projetada à frente na rota prevista do suspeito ($v \cdot \Delta t_{\text{lookahead}}$).
  - Busca ultrapassar o infrator ou posicionar-se em cruzamentos adiante na `TrafficWaypointNetwork` para forçar frenagem ou bloqueio.
* **`PerimeterSearch` (Busca Perimetral)**:
  - Papel temporário ativado quando o contato visual é perdido. Cada guarnição investiga um quadrante distinto ($+X, -X, +Z, -Z$) ao redor da LKP.

---

## 3. Dinâmica de Perseguição e Rádio COPOM

### 3.1. Compartilhamento Global de Avistamento
* Qualquer viatura que mantiver contato visual direto com o jogador aciona `ReportVisualContact(reportingUnit, targetPos)`.
* O `PolicePursuitCoordinator` atualiza `GlobalLastKnownPosition` e retransmite o alvo a todas as guarnições ativas, permitindo que viaturas fora da linha de visada continuem em percurso tático de aproximação.

### 3.2. Busca Coordenada e Evasão
* Quando todas as viaturas perdem contato visual:
  - O estado do sistema entra em contagem regressiva de busca perimetral (`searchTimeout = 14s`).
  - Viaturas são distribuídas em quadrantes distintos ao redor da LKP, cobrindo um raio de $35\text{ m}$.
  - Se qualquer viatura recuperar a visão antes do esgotamento do tempo, a perseguição é restabelecida imediatamente com reaproveitamento dos papéis táticos.
  - Se o tempo expirar sem reavistamento, a perseguição é encerrada por **Evasão** (`OnPursuitEvaded`), viaturas desligam sirenes e retornam a `ReturningToPatrol`, permitindo que o `WantedLevelManager` inicie o decay de procurado.

### 3.3. Abordagem e Prisão Coordenada (Busted)
* Ao consumar a abordagem em velocidade controlada com tempo contínuo de retenção (`arrestDurationRequired`), a viatura abordadora dispara `ReportBusted`.
* O coordenador transmite a ordem de prisão para todas as unidades participantes, que cessam a perseguição, desligam sirenes e retornam ao patrulhamento preventivo.

---

## 4. Otimizações para Android e Zero GC

1. **Limite Estrito de Unidades**:
   - `maxPursuitUnits = 4` ativas simultaneamente, evitando saturação de CPU em aparelhos móveis.
2. **Cálculos Vetoriais Otimizados**:
   - Comparações de distância executadas exclusivamente via `sqrMagnitude`.
   - Offsets táticos e previsões calculados com aritmética vetorial simples em estruturas por valor (`Vector3`).
3. **Comunicação por Eventos Reativos**:
   - `Action<bool>`, `Action<PolicePursuitController, PoliceTacticalRole>` e `Action<Vector3>`, sem uso de strings, reflection ou `SendMessage`.

---

## 5. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `PoliceTacticalRole.cs` | `10000000000000000000000000000096` | Enum de papéis táticos policiais |
| `PolicePursuitCoordinator.cs` | `10000000000000000000000000000097` | Orquestrador central multi-viatura e rádio COPOM |
| `PolicePursuitController.cs` | `10000000000000000000000000000076` | Controlador individual integrado com papéis táticos |
| `UrbanTrafficTestScene.unity` | N/A | Entidade de coordenação integrada na cena de testes |
