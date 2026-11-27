# Sistema de Ambiente e Quarteirões Procedurais (SYS_PROCEDURAL_ENVIRONMENT)

---
system_id: SYS_PROCEDURAL_ENVIRONMENT
parent_system: SYS-19 (Mundo Aberto)
status: IMPLEMENTADO_EM_FASE_5_7
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_WORLD]]
  - [[SYS_WORLD_STREAMING]]
  - [[SYS_TRAFFIC_NAVIGATION]]
---

## 1. Visão Geral

O **Sistema de Ambiente Procedural** (`ProceduralEnvironmentGenerator`) possibilita a construção modular de quarteirões urbanos, calçadas, vias e lotes comerciais ou residenciais com base em uma semente inteira determinística (`worldSeed`).

A mesma semente sempre gera rigorosamente a mesma distribuição de edifícios, alturas de gabarito e posicionamento de mobiliário urbano, permitindo um mundo expansível antes da inserção final da cidade real de Araguari.

---

## 2. Configurações Orientadas a Dados (`ProceduralBlockConfig`)

* **Quarteirão Residencial (`ProceduralBlockConfig_Residential.asset`)**:
  - Dimensões: $80\text{ m} \times 80\text{ m}$.
  - Edifícios com gabarito de $6\text{ m}$ a $18\text{ m}$.
  - Calçadas de $3\text{ m}$ e postes de iluminação espaçados a cada $25\text{ m}$.
* **Quarteirão Comercial (`ProceduralBlockConfig_Commercial.asset`)**:
  - Dimensões: $80\text{ m} \times 80\text{ m}$.
  - Edifícios e lojas com gabarito de $12\text{ m}$ a $36\text{ m}$.
  - Calçadas de $4\text{ m}$, vias de $16\text{ m}$ e hidrantes urbanos.

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `ProceduralBlockConfig.cs` | `1000000000000000000000000000009f` | ScriptableObject com parâmetros de quarteirão |
| `ProceduralEnvironmentGenerator.cs` | `100000000000000000000000000000a0` | Gerador determinístico baseado em semente |
| `ProceduralBlockConfig_Residential.asset` | `200000000000000000000000000000a0` | Preset residencial |
| `ProceduralBlockConfig_Commercial.asset` | `200000000000000000000000000000a1` | Preset comercial |
