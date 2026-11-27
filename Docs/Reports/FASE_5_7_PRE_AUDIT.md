# Relatório de Pré-Auditoria Técnica — Fase 5.7: Construção do Mundo Urbano e Ambiente de Jogo

---
auditoria_id: FASE_5_7_PRE_AUDIT
data: 2026-11-23
projeto: Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)
status: APROVADO_PARA_EXECUCAO
autor: AI Senior Software Engineer & Unity Systems Architect
---

## 1. Contexto e Objetivo da Fase 5.7

A **Fase 5.7** do Projeto Grau 2 tem como objetivo central desenvolver a **infraestrutura de mundo aberto expansível** com foco em alta performance para dispositivos Android.

Conforme orientação explícita do usuário:
> **IMPORTANTE**: Ainda NÃO implementar a cidade real de Araguari. Primeiro devemos finalizar a estrutura geral do jogo. A cidade real será uma etapa posterior.

O foco é criar a arquitetura técnica que tornará viável a futura implementação de Araguari:
1. **World Streaming System**: Divisão do mundo em células (Chunks/Grid), carregamento prioritário próximo ao jogador e descarregamento de áreas distantes sem congelamentos.
2. **Procedural Environment System**: Geração modular determinística de quarteirões, calçadas, vias e lotes baseada em sementes numéricas (`seed`).
3. **World Population System**: Pedestres NPC com Object Pooling, comportamento de circulação em calçadas e integração com cruzamentos de tráfego.
4. **World LOD System**: Sistema hierárquico de 3 níveis de detalhe (LOD 0, 1, 2) e culling de renderers/colliders para objetos urbanos.
5. **World Optimization Manager**: Orquestrador global de orçamento de memória, controle de draw calls e atualização escalonada (time-slicing) para manter 60 FPS no Android.

---

## 2. Inventário de Dependências e Integrações Existentes

* **`TrafficNavigationManager.cs` (`GUID: 1000...46`)**: Rede de nós e semáforos existente; os novos quarteirões se conectam aos nós vetoriais.
* **`AIVehiclePool.cs` (`GUID: 1000...58`)**: Padrão de pooling reaproveitado para pedestres (`PedestrianPool`).
* **`TrafficLODManager.cs` (`GUID: 1000...59`)**: Padrão de LOD por distância reaproveitado e integrado ao `WorldLODGroup` e `WorldOptimizationManager`.
* **`MotorcycleController.cs` (`GUID: 1000...02`)**: Posição do jogador utilizada como âncora de streaming espacial.
* **`WorldEventManager.cs` (`GUID: 1000...94`)**: Ocorrências viárias e bloqueios instanciados dentro dos chunks ativos.

---

## 3. Matriz de Componentes Novos e GUIDs Determinísticos

| Componente | Tipo | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- | :--- |
| `WorldChunkCoord.cs` | Struct C# | `1000000000000000000000000000009c` | Coordenada inteira discreta `(x, z)` do grid de células |
| `WorldChunk.cs` | Script MonoBehaviour | `1000000000000000000000000000009d` | Representação de uma célula do mundo com ativação/desativação de objetos |
| `WorldStreamer.cs` | Script MonoBehaviour | `1000000000000000000000000000009e` | Gerenciador de streaming dinâmico por raio ao redor do jogador |
| `ProceduralBlockConfig.cs` | ScriptableObject | `1000000000000000000000000000009f` | Configuração de módulos de quarteirão, calçadas e edifícios |
| `ProceduralEnvironmentGenerator.cs` | Script MonoBehaviour | `100000000000000000000000000000a0` | Gerador modular determinístico baseado em semente (`seed`) |
| `PedestrianController.cs` | Script MonoBehaviour | `100000000000000000000000000000a1` | IA leve de pedestre para calçadas e travessias |
| `PedestrianPool.cs` | Script MonoBehaviour | `100000000000000000000000000000a2` | Object Pool pré-alocado para pedestres (Zero GC) |
| `WorldPopulationManager.cs` | Script MonoBehaviour | `100000000000000000000000000000a3` | Controle de densidade populacional e culling de pedestres distantes |
| `WorldLODGroup.cs` | Script MonoBehaviour | `100000000000000000000000000000a4` | Controle de 3 níveis de LOD e culling de geometria/colisores |
| `WorldOptimizationManager.cs` | Script MonoBehaviour | `100000000000000000000000000000a5` | Governador de orçamento de memória, draw calls e time-slicing Android |
| `ProceduralBlockConfig_Residential.asset` | Asset ScriptableObject | `200000000000000000000000000000a0` | Preset de quarteirão residencial |
| `ProceduralBlockConfig_Commercial.asset` | Asset ScriptableObject | `200000000000000000000000000000a1` | Preset de quarteirão comercial |

---

## 4. Diretrizes de Otimização para Android (Zero GC e 60 FPS)

1. **Streaming Baseado em Ativação de Objetos**:
   - Em vez de destruir e recriar meshes em tempo de execução, os chunks utilizam pooling de raízes e ativação (`SetActive(true/false)`) suave sem picos de alocação de memória no GC.
2. **Time-Slicing**:
   - As distâncias de streaming e de LOD não são recalculadas para todos os objetos a cada frame. O `WorldOptimizationManager` divide o universo de objetos em lotes escalonados distribuídos ao longo de vários frames.
3. **Culling Estrito de Pedestres**:
   - Pedestres a mais de $60\text{ m}$ do jogador são desativados e devolvidos ao pool.
   - Limite de pedestres ativos simultâneos configurado entre 15 e 30 para proteção de dispositivos móveis de entrada.
4. **LOD de Física e Colisores**:
   - Prédios e estruturas em LOD 2 ($> 90\text{ m}$) têm seus `MeshCollider` desativados, liberando processamento do PhysX.

---

## 5. Plano de Testes Automatizados (`test_world_infrastructure.py`)

A nova suíte verificará:
1. Integridade de scripts e metadados GUID determinísticos da Fase 5.7.
2. Presença de entidades de streaming, geração e otimização na cena.
3. Teste de semente procedural: mesma seed gera quarteirões e lotes com as mesmas posições e tipos.
4. Teste de streaming: chunks a $< 100\text{ m}$ carregam; chunks a $> 250\text{ m}$ descarregam.
5. Teste de transições de LOD: LOD 0 ($< 40\text{ m}$), LOD 1 ($40-90\text{ m}$), LOD 2 ($90-160\text{ m}$), Culling ($> 160\text{ m}$).
6. Teste de pooling de pedestres: spawn e despawn sem novas instâncias.
7. Teste de time-slicing e orçamento de entidades no `WorldOptimizationManager`.
8. Regressão estrita de 100% das 6 suítes anteriores (5.1 a 5.6 = 361 testes).
