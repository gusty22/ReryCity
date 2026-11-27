# Relatório de Pré-Auditoria Técnica — Etapa 5.2
## Sistema de Veículos Civis NPC, Física Veicular e Inteligência de Tráfego

---
documento: FASE_5_2_PRE_AUDIT.md
data: 2026-11-23
versao: 1.0.0
responsavel: AI Senior Software Engineer & Unity Systems Architect
status: APROVADO PARA IMPLEMENTACAO
---

## 1. Visão Geral e Contexto da Auditoria

Em cumprimento ao protocolo de engenharia e às diretrizes do projeto (`PROJECT_RULES.md`, `CODING_STANDARDS.md`, `HANDOFF_PROTOCOL.md`, `CONTEXT.md`), foi realizada a auditoria técnica prévia antes da implementação da **Etapa 5.2 — Sistema de Veículos Civis NPC**.

O objetivo desta auditoria é assegurar que a nova camada de inteligência e física veicular NPC seja construída estritamente sobre a infraestrutura existente (Fases 0 a 4 e Etapa 5.1), sem duplicidades, com respeito à filosofia *Clean-Room* (ADR-004), desacoplamento físico (ADR-002), parametrização via `ScriptableObjects` (ADR-003) e viabilidade móvel para 60 FPS (Android).

---

## 2. Inventário e Auditoria dos Componentes Existentes (Etapa 5.1)

Todos os scripts da malha viária criados na Etapa 5.1 foram validados em `Assets/Scripts/Traffic/`:

| Script | GUID | Status de Auditoria | Capacidade de Reutilização Direta |
| :--- | :--- | :--- | :--- |
| `TrafficWaypoint.cs` | `10000000000000000000000000000040` | Íntegro | Fornece posições vetoriais, limites de velocidade, detecção de linha de parada (*StopLine*) e semáforos associados aos sensores frontais de IA. |
| `TrafficLane.cs` | `10000000000000000000000000000041` | Íntegro | Permite que o NPC siga faixas sequenciais, consulte o sentido da via e execute manobras de mudança/ultrapassagem consultando `leftAdjacentLane` e `rightAdjacentLane`. |
| `TrafficNode.cs` | `10000000000000000000000000000042` | Íntegro | Permite bifurcações de rota e seleção estocástica de saídas ponderadas para diversificação de fluxo de NPCs. |
| `TrafficSignal.cs` | `10000000000000000000000000000043` | Íntegro | Permite que o sensor de IA do NPC consulte `IsRed`, `IsYellow`, `IsGreen` e `RemainingTimeInState` para decisão antecipada de frenagem. |
| `TrafficIntersection.cs` | `10000000000000000000000000000044` | Íntegro | Governa aproximações e conflitos viários através de `CanEnterIntersection()`, `RegisterVehicleEntry()` e `RegisterVehicleExit()`. |
| `TrafficWaypointNetwork.cs`| `10000000000000000000000000000045` | Íntegro | Fornece indexação topológica global e busca espacial euclidiana (`GetNearestWaypoint`, `GetNearestLane`). |
| `TrafficNavigationManager.cs`| `10000000000000000000000000000046` | Íntegro | Singleton que atua como barramento de consultas rápidas para os controllers de IA e renderização de Gizmos no editor. |

**Diagnóstico**: 100% dos componentes de navegação estão consolidados, estáveis e prontos para consumo pela Etapa 5.2. Nenhuma modificação estrutural na malha viária se faz necessária.

---

## 3. Auditoria da Cena de Testes Atual

- **Arquivo**: `Assets/Scenes/UrbanTrafficTestScene.unity` (`GUID: 30000000000000000000000000000005`).
- **Estado Atual**: Contém o cruzamento urbano em 4 vias (`Intersection_Centro_4Way`), faixas norte-sul e leste-oeste, semáforos temporizados de 4 fases, `TrafficWaypointNetwork`, `TrafficNavigationManager`, iluminação direcional e plano de asfalto.
- **Necessidades para a Etapa 5.2**:
  1. Instanciar ou posicionar instâncias de veículos NPC (carros populares, sedãs, motos civis, caminhões).
  2. Inserir representações proxy com *BoxCollider* e geometrias provisórias (dada a ausência de malhas 3D proprietárias soltas no repositório).
  3. Adicionar obstáculos e uma representação física da motocicleta do jogador para validar desvios, frenagens de emergência e respeito a distâncias de segurança.

---

## 4. Plano Arquitetural da Etapa 5.2

### 4.1. Camada de Dados (ScriptableObjects & Enums)
* `AIVehicleType.cs` (`GUID: 1000...50`): Categorias de veículos (CivilMotorcycle, EconomyCar, Sedan, SUV, Pickup, Truck, Bus, PoliceCruiser).
* `AIDriverProfile.cs` (`GUID: 1000...51`): ScriptableObject que define o perfil do motorista (Cautious, Normal, Aggressive), distâncias de seguimento ($d_{safe}$), agressividade de aceleração/frenagem e propensão a ultrapassagens.
* `AIVehicleData.cs` (`GUID: 1000...52`): ScriptableObject com física base do veículo: massa, dimensões do collider, velocidade máxima, torque de aceleração, potência de frenagem, raio de giro e coeficientes de arrasto.

### 4.2. Controladores Modulares de Física e Inteligência
* `AIVehicleSensor.cs` (`GUID: 1000...54`): Sensores de detecção frontais, laterais e traseiros via `Raycast` e `SphereCast` com cadência adaptativa (LOD) e exclusão dos próprios colliders.
* `AIVehicleMotorController.cs` (`GUID: 1000...55`): Força motriz progressiva aplicada via `Rigidbody.AddForce` no eixo longitudinal, respeitando velocidade limite e massa.
* `AIVehicleSteeringController.cs` (`GUID: 1000...56`): Esterçamento e rotação angular proporcional à velocidade (`Rigidbody.MoveRotation` / `AddTorque`), com inclinação visual (*lean banking*) procedural para motocicletas.
* `AIVehicleBrakeController.cs` (`GUID: 1000...57`): Força contrária de atrito e frenagem física progressiva, sem travamento artificial instantâneo.
* `AIVehicleStateMachine.cs` (`GUID: 1000...53`): Máquina de estados com 10 estados finitos (`Idle`, `Cruise`, `FollowVehicle`, `Brake`, `StopAtSignal`, `IntersectionWaiting`, `LaneChanging`, `Overtaking`, `AvoidObstacle`, `EmergencyStop`) e controle de histerese temporal.
* `AIVehicleController.cs` (`GUID: 1000...58`): Orquestrador central que agrega os módulos, integra-se ao `TrafficNavigationManager` e gerencia o ciclo de vida do veículo.

### 4.3. Gerenciamento e Otimização para Android (60 FPS)
* `AIVehiclePool.cs` (`GUID: 1000...59`): Sistema de *Object Pooling* com pré-alocação estática por categoria para evitar *Garbage Collection* no Android.
* `TrafficLODManager.cs` (`GUID: 1000...60`): Gerenciador de níveis de detalhe comportamental:
  - LOD 0 (< 30m): Sensores a 20 Hz, física completa `FixedUpdate`.
  - LOD 1 (30m - 70m): Sensores a 5 Hz, cinemática simplificada.
  - LOD 2 (> 70m): Atualização em lote a 1 Hz ou desativação para o pool.
* `TrafficSpawnManager.cs` (`GUID: 1000...61`): Spawner perimetral ao redor do jogador/câmera com limite rígido de NPCs simultâneos e verificação de desobstrução de faixa.

---

## 5. Matriz de Conformidade e Restrições Técnicas

| Requisito | Solução Técnica | Verificação |
| :--- | :--- | :--- |
| **Zero GC em Ciclos Críticos** | Cache de RaycastHit, reaproveitamento de arrays e pooling de veículos | Estática e execução de teste |
| **Física Realista sem `Translate`** | Aplicação estrita de `Rigidbody.AddForce` e controle de torque/velocidade em `FixedUpdate` | Inspeção de código |
| **Modelos 3D Ausentes** | Estruturas proxy com *BoxCollider* e cubos/cápsulas primitivas paramétricas | Validação de cena |
| **Detecção do Jogador** | Camada/Tag específica com prioridade de parada e frenagem de emergência | Simulação de aproximação |
| **Controle de Semáforos** | Integração direta com `TrafficSignal.IsRed` na linha de retenção | Simulação de tráfego |

---

## 6. Conclusão da Auditoria

A infraestrutura atual está **100% pronta e aprovada** para receber a Etapa 5.2.  
Nenhuma dependência impeditiva foi encontrada. A implementação pode prosseguir imediatamente.
