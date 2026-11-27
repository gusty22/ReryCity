# Sistema de Otimização e LOD do Mundo Urbano (SYS_WORLD_OPTIMIZATION)

---
system_id: SYS_WORLD_OPTIMIZATION
parent_system: SYS-19 (Mundo Aberto)
status: IMPLEMENTADO_EM_FASE_5_7
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_WORLD]]
  - [[SYS_WORLD_STREAMING]]
  - [[SYS_TRAFFIC_OPTIMIZATION]]
---

## 1. Visão Geral

O **Sistema de Otimização e LOD do Mundo** (`WorldOptimizationManager` e `WorldLODGroup`) é o governador de desempenho de mundo aberto para dispositivos Android, assegurando taxa de quadros estável de **60 FPS** em processadores móveis.

---

## 2. Níveis de Detalhe e Culling Físico (`WorldLODGroup`)

* **LOD 0 ($< 35\text{ m}$)**:
  - Malha geométrica de alta fidelidade e renderizadores ativos.
  - Colisores físicos ativos para impactos de motocicleta e interação com o mundo.
* **LOD 1 ($35\text{ m} - 80\text{ m}$)**:
  - Geometria intermediária. Colisores mantidos ativos para detecção de colisão veicular.
* **LOD 2 ($80\text{ m} - 150\text{ m}$)**:
  - Modelo simplificado em bloco.
  - **Colisores PhysX desativados**: Alivia drasticamente a árvore de colisão da física do Unity em prédios que estão longe da interação do jogador.
* **Culled ($> 150\text{ m}$)**:
  - Renderizadores e colisores completamente desligados.

---

## 3. Time-Slicing e Orçamento de Entidades (`WorldOptimizationManager`)

* **Time-Slicing**:
  - Os elementos urbanos são distribuídos em 4 lotes (`timeSliceBuckets = 4`), avaliando apenas um lote por frame. Isso elimina os picos de processamento na CPU mobile.
* **Tetos de Densidade no Android**:
  - Carros Civis Ativos: Máx. 12
  - Pedestres Ativos: Máx. 15
  - Viaturas Policiais: Máx. 4
  - Chunks Carregados: Máx. 9 (Grade $3 \times 3$)

---

## 4. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `WorldLODGroup.cs` | `100000000000000000000000000000a4` | Controle de 3 níveis de LOD e desativação de colisores |
| `WorldOptimizationManager.cs` | `100000000000000000000000000000a5` | Gerenciador global de orçamento e time-slicing |
