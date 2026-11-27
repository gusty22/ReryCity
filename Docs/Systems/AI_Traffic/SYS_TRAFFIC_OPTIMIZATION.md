# SYS_TRAFFIC_OPTIMIZATION — Otimização Móvel, Object Pooling e LOD

---
type: system_specification
id: SYS-13.5
status: IMPLEMENTADO_EM_ETAPA_5_2
created: 2026-11-23
phase: 5
tags:
  - mobile_optimization
  - object_pool
  - lod
  - android_60fps
dependencies:
  - SYS_AI_VEHICLES
  - ADR_001_UNITY6_URP
---

## 1. Estratégia de Desempenho para Android (60 FPS)

A execução simultânea de dezenas de veículos com inteligência artificial e física em dispositivos intermediários exige técnicas rígidas de otimização:

### 1.1. Object Pooling (`AIVehiclePool.cs`)
* Pré-alocação estática no `Awake` de até 3 instâncias por categoria.
* Reutilização de instâncias via `ResetVehicle()`, eliminando chamadas de `Instantiate()` e `Destroy()` durante o gameplay.
* Zero alocação de lixo no heap (*Zero GC*).

### 1.2. Níveis de Detalhe Comportamental (`TrafficLODManager.cs`)
* **LOD 0 (< 35m)**: Sensores a 20 Hz (intervalo 0.05s), física completa com interpolação `FixedUpdate`.
* **LOD 1 (35m a 75m)**: Sensores a 5 Hz (intervalo 0.20s), física simplificada.
* **LOD 2 (> 75m)**: Sensores a 1 Hz (intervalo 1.00s).
* **Despawn (> 120m)**: Retorno imediato ao pool de inativos.

### 1.3. Escalonamento Temporal Distribuído (*Time-Slicing*)
* As avaliações de LOD não ocorrem para todos os veículos no mesmo frame.
* O `TrafficLODManager` processa em lotes de 4 veículos por frame (`BATCH_SIZE = 4`), espalhando a carga de CPU uniformemente.
