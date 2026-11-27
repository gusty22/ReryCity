# SYS_TRAFFIC_NAVIGATION — Infraestrutura de Navegação Urbana e Tráfego

---
type: system_specification
id: SYS-13
status: IMPLEMENTADO_EM_ETAPA_5_1
created: 2026-11-23
phase: 5
tags:
  - traffic
  - navigation
  - waypoints
  - intersections
  - signals
  - mobile_optimized
dependencies:
  - ADR_001_UNITY6_URP
  - ADR_002_CLEAN_ARCHITECTURE_PHYSICS
  - ADR_004_CLEAN_ROOM_RECONSTRUCTION
---

## 1. Visão Geral do Sistema

O sistema de **Navegação Urbana de Tráfego** fornece a malha viária vetorial para circulação determinística de veículos civis (NPCs), viaturas policiais e pedestres.
Diferente de sistemas que dependem de malhas de navegação dinâmicas pesadas (*Unity NavMesh*), que geram sobrecarga de CPU e alto consumo de bateria em plataformas móveis (Android), esta solução adota uma **rede de waypoints vetoriais desacoplada e ultraleve**.

A malha permite que múltiplos veículos trafeguem simultaneamente a 60 FPS estáveis mesmo em dispositivos intermediários, com custo de processamento inferior a 0.2ms por quadro.

---

## 2. Componentes e Arquitetura

### 2.1. TrafficWaypoint (`TrafficWaypoint.cs`)
- **Papel**: Ponto geométrico vetorial individual ao longo de uma via urbana.
- **Atributos**:
  - `Position` e `Forward`: Direção de deslocamento do fluxo viário.
  - `speedLimitKmh`: Velocidade regulamentada no trecho (ex: 40 km/h a 80 km/h).
  - `laneWidth`: Largura útil da faixa (padrão 3.5m).
  - `isStopLine`: Indica se o ponto atua como linha de retenção de semáforo ou cruzamento.
  - `nextWaypoints` e `previousWaypoints`: Relações topológicas direcionais de fluxo.
  - `associatedSignal`: Vínculo direto com o semáforo que governa este ponto de retenção.
  - `parentLane`: Referência à faixa a que pertence o waypoint.

### 2.2. TrafficLane (`TrafficLane.cs`)
- **Papel**: Faixa de rolamento contínua composta por uma sequência ordenada de waypoints.
- **Atributos**:
  - `laneId`: Identificador único (ex: `lane_north_in_0`).
  - `laneIndex`: Índice da faixa (0 = faixa da direita/lenta, 1 = faixa da esquerda/ultrapassagem).
  - `direction`: Sentido de tráfego (`Forward` ou `Backward` para pistas de mão dupla).
  - `leftAdjacentLane` e `rightAdjacentLane`: Faixas vizinhas para manobras de ultrapassagem e mudança de faixa.
  - `GetNearestWaypoint(Vector3 position)`: Busca euclidiana rápida do waypoint mais próximo.

### 2.3. TrafficNode (`TrafficNode.cs`)
- **Papel**: Nó de convergência ou divergência topológica (bifurcações, confluências ou entroncamentos).
- **Atributos**:
  - `nodeType`: `Standard`, `Diverge` (1 entrada -> N saídas), `Merge` (N entradas -> 1 saída), `Junction`.
  - `nodeRadius`: Raio de influência/área de decisão do nó.
  - `GetRandomOutgoingLane()`: Seleção de rota probabilística com pesos ponderados para NPCs.
  - `LinkLanesWaypoints()`: Conexão automática de continuidade viária.

### 2.4. TrafficSignal (`TrafficSignal.cs`)
- **Papel**: Controlador de semáforos urbanos com ciclo temporizado de 4 fases.
- **Ciclo de Estados**:
  1. `Green`: Tráfego liberado (ex: 20s).
  2. `Yellow`: Atenção e desaceleração (ex: 3.5s).
  3. `AllRed`: Intervalo de segurança / esvaziamento total do cruzamento (ex: 2s).
  4. `Red`: Retenção obrigatória (ex: 24s).
- **Atributos**:
  - `phaseGroup`: Grupo de sincronização (ex: Grupo 0 = Norte-Sul, Grupo 1 = Leste-Oeste).
  - `IsGreen`, `IsYellow`, `IsRed`: Consultas booleanas de estado.
  - `RemainingTimeInState`: Tempo restante na fase atual para tomada de decisão antecipada por IA.

### 2.5. TrafficIntersection (`TrafficIntersection.cs`)
- **Papel**: Gerenciador de cruzamento urbano com coordenação semafórica e regras de trânsito.
- **Regras de Prioridade**:
  - `TrafficLightControlled`: Controlado por semáforos com sincronização de fases antagônicas.
  - `PriorityRoad`: Via preferencial (faixas secundárias cedem a preferência se houver tráfego).
  - `AllWayStop`: Parada obrigatória (Stop Sign) com liberação sequencial por ausência de ocupantes.
  - `Roundabout`: Rotatória (preferência de quem já circula no anel central).
- **Prevenção de Colisão e Deadlock**:
  - `occupyingVehicles`: Rastreamento ativo de veículos dentro da área de conflito (`conflictRadius`).
  - `CanEnterIntersection(Transform vehicle, TrafficLane lane)`: Impede a invasão do cruzamento caso esteja obstruído ou se o semáforo estiver fechado.

### 2.6. TrafficWaypointNetwork (`TrafficWaypointNetwork.cs`)
- **Papel**: Contêiner hierárquico de todo o grafo viário de um distrito ou cidade.
- **Funcionalidades**:
  - Registro de faixas, nós e cruzamentos.
  - Varredura automática (`ScanAndRegisterChildren`).
  - Validação de integridade topológica (`ValidateNetwork`).
  - Consultas espaciais globais (`GetNearestWaypoint`, `GetNearestLane`).

### 2.7. TrafficNavigationManager (`TrafficNavigationManager.cs`)
- **Papel**: Singleton de alto nível para acesso unificado por IAs de NPCs, viaturas policiais e depuração visual.
- **Visualização Gizmos**:
  - Trajetórias de faixas (linhas ciano com vetores direcionais).
  - Waypoints (esferas verdes).
  - Linhas de retenção (faixas vermelhas transversais).
  - Cruzamentos (esferas de conflito roxas translúcidas).
  - Semáforos (lâmpadas com cores dinâmicas de estado).

---

## 3. Integração com as Próximas Etapas

- **Etapa 5.2 (Veículos Civis NPC)**: O controlador de veículos NPC consumirá o `TrafficNavigationManager` para obter waypoints, calcular frenagem diante de semáforos vermelhos e mudar de faixa.
- **Etapa 5.3 (IA Policial)**: Viaturas de perseguição utilizarão os `TrafficNodes` e a rede vetorial para interceptar o jogador e desviar do fluxo civil.
- **Etapa 5.4 (Sistema de Perseguição e Ocorrências)**: Geração de bloqueios viários através do fechamento forçado de faixas na rede.

---

## 4. Testes e Validação

- Testes automatizados executados via script de simulação algorítmica `test_traffic_navigation.py`:
  - Integridade de 7 scripts C# e 7 arquivos `.meta` com GUIDs determinísticos: **100% aprovado**.
  - Validação da cena `UrbanTrafficTestScene.unity` e hierarquia de componentes: **100% aprovado**.
  - Simulação de proximidade euclidiana e limites de velocidade de waypoints: **100% aprovado**.
  - Ciclo de 4 fases de semáforo com fase de segurança All-Red: **100% aprovado**.
  - Prevenção de deadlock e respeito a prioridades em cruzamentos: **100% aprovado**.
  - Total: **43/43 testes aprovados**.
