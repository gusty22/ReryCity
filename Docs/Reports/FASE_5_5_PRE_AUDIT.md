# Relatório de Pré-Auditoria Técnica — Fase 5.5: Integração Avançada de Perseguições Policiais

---
auditoria_id: FASE_5_5_PRE_AUDIT
data: 2026-11-23
projeto: Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)
status: APROVADO_PARA_EXECUCAO
autor: AI Senior Software Engineer & Unity Systems Architect
---

## 1. Contexto e Objetivo da Fase 5.5

A **Fase 5.5** expande e consolida as capacidades policiais introduzidas na Fase 5.3 e a infraestrutura de ocorrências da Fase 5.4, criando uma **camada de coordenação tática multi-unidade** com perseguições realistas, compartilhamento de rádio em tempo real, diferenciação operacional entre viaturas 4x4 e motos ROCAM, busca coordenada na LKP e respeito rigoroso às restrições de desempenho da plataforma Android.

---

## 2. Inventário de Dependências e Integrações

### 2.1. Dependências Diretas Existentes
* **`PoliceAIStateMachine.cs` (`GUID: 1000...73`)**: Máquina com 8 estados (`Patrol`, `Suspicious`, `Investigating`, `Pursuit`, `Intercepting`, `Arresting`, `LostTarget`, `ReturningToPatrol`).
* **`PolicePursuitController.cs` (`GUID: 1000...76`)**: Condução física, percepção sensorial e controle de abordagem.
* **`PoliceVehicleData.cs` (`GUID: 1000...71`)**: Tipologia veicular (`CruiserSUV`, `RocamMotorcycle`, `HighwayInterceptor`).
* **`PoliceAIData.cs` (`GUID: 1000...72`)**: Parâmetros de visão, busca e abordagem.
* **`WantedLevelManager.cs` (`GUID: 1000...75`)**: Escalonamento de procurado de 0 a 5 estrelas e decay de evasão.
* **`TrafficViolationDetector.cs` (`GUID: 1000...74`)**: Detecção de infrações (velocidade, grau, contramão, sinal vermelho).
* **`TrafficWaypointNetwork.cs` (`GUID: 1000...45`)**: Malha vetorial para reavaliação de rotas e interceptações.
* **`PoliceDispatchManager.cs` (`GUID: 1000...95`)**: Central de chamados de sinistros e ocorrências.
* **`PoliceHUD.cs` (`GUID: 1000...77`)**: Interface de alerta policial e barra de abordagem.

---

## 3. Matriz de Componentes Novos e Modificados

| Componente | Tipo | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- | :--- |
| `PoliceTacticalRole.cs` | Script (Enum) | `10000000000000000000000000000096` | Papéis táticos em perseguição: `LeadPursuer`, `FlankerLeft`, `FlankerRight`, `Interceptor`, `PerimeterSearch`, `Support` |
| `PolicePursuitCoordinator.cs` | Script (MonoBehaviour) | `10000000000000000000000000000097` | Orquestrador central multi-viatura: rádio COPOM, compartilhamento de LKP, atribuição de papéis, busca setorial |
| `PolicePursuitController.cs` | Script (Modificado) | `10000000000000000000000000000076` | Integração com o coordenador, resposta ao papel tático e busca setorial na LKP |
| `UrbanTrafficTestScene.unity` | Cena (Modificada) | N/A | Adição do GameObject `PolicePursuitCoordinator` e configuração na hierarquia |

---

## 4. Diretrizes de Otimização para Android (Zero GC)

1. **Estruturas de Dados Sem Alocação**:
   * O `PolicePursuitCoordinator` utiliza listas e arrays pré-alocados de tamanho fixo (`maxActivePursuitUnits = 4`).
   * Iterações em `Update` e `FixedUpdate` não utilizam LINQ ou alocações dinâmicas de coleções.
2. **Cálculo de Distância Eficiente**:
   * Utilização exclusiva de `sqrMagnitude` para comparações de proximidade e alocação de viatura líder.
3. **Busca Espacial Sem NavMesh**:
   * Reavaliação de rotas e pontos de interceptação baseada puramente na geometria vetorial da `TrafficWaypointNetwork` e projeção linear de vetor velocidade.
4. **Broadcast de Visão e Rádio**:
   * Compartilhamento por eventos C# desacoplados (`Action<Vector3>`, `Action<PolicePursuitController, PoliceTacticalRole>`), evitando overhead de mensagens por reflexão (`SendMessage`).

---

## 5. Plano de Testes Automatizados (`test_police_pursuit_advanced.py`)

A nova suíte verificará formalmente:
1. Integridade de scripts e metadados GUID determinísticos da Fase 5.5.
2. Presença do `PolicePursuitCoordinator` na cena `UrbanTrafficTestScene.unity`.
3. Atribuição diferenciada de papéis táticos (ROCAM como `LeadPursuer`, Cruiser como `Interceptor` ou `Flanker`).
4. Compartilhamento em tempo real de LKP e avistamento entre guarnições policiais.
5. Busca setorial coordenada na perda de contato visual (sem aglomeração no mesmo ponto).
6. Escalonamento de viaturas ativas conforme Wanted Level (1 a 5 estrelas).
7. Interceptação tática com corte de rota via `TrafficWaypointNetwork`.
8. Encerramento coordenado após captura (Busted) ou fuga (Evaded).
9. Regressão estrita de 100% das suítes anteriores (5.1, 5.2, 5.3, 5.4).
