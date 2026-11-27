# Sistema de Streaming de Mundo Aberto (SYS_WORLD_STREAMING)

---
system_id: SYS_WORLD_STREAMING
parent_system: SYS-19 (Mundo Aberto)
status: IMPLEMENTADO_EM_FASE_5_7
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_WORLD]]
  - [[SYS_WORLD_OPTIMIZATION]]
  - [[SYS_PROCEDURAL_ENVIRONMENT]]
---

## 1. Visão Geral

O **Sistema de Streaming de Mundo Aberto** (`WorldStreamer`) é a infraestrutura central responsável por particionar o ambiente de jogo em células bidimensionais discretas (`WorldChunk` e `WorldChunkCoord`), carregando preventivamente as áreas no raio do jogador e descarregando células distantes sem congelamento de quadros (*stutter-free streaming*).

---

## 2. Princípios de Operação

### 2.1. Grade e Chunks Espaciais
* Cada célula possui tamanho lateral configurável (`chunkSize = 100m`).
* A conversão de posições do mundo tridimensional para coordenadas de grid é executada de forma determinística por `WorldChunkCoord.FromWorldPosition(pos, chunkSize)` sem alocação no heap.
* Raio de carregamento padrão: 1 chunk (grade ativa $3 \times 3 = 9$ chunks ao redor da posição do jogador).

### 2.2. Descarregamento por Distância e Proteção de Memória
* Células que ultrapassam a distância limite (`unloadDistance = 220m`) são descarregadas automaticamente (`chunk.Unload()`).
* O descarregamento opera por desativação hierárquica (`SetActive(false)`), reaproveitando as estruturas em memória sem invocar o coletor de lixo do Unity (Zero GC).

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `WorldChunkCoord.cs` | `1000000000000000000000000000009c` | Struct de coordenadas inteiras de grade |
| `WorldChunk.cs` | `1000000000000000000000000000009d` | Célula espacial com controle de entidades |
| `WorldStreamer.cs` | `1000000000000000000000000000009e` | Gerenciador global de streaming por raio |
