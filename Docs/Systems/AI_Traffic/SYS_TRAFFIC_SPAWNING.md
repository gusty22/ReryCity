# SYS_TRAFFIC_SPAWNING — Geração e Dispersão Perimetral de Tráfego

---
type: system_specification
id: SYS-13.4
status: IMPLEMENTADO_EM_ETAPA_5_2
created: 2026-11-23
phase: 5
tags:
  - traffic_spawning
  - object_pool
  - dynamic_density
dependencies:
  - SYS_AI_VEHICLES
  - SYS_TRAFFIC_NAVIGATION
---

## 1. Visão Geral

O `TrafficSpawnManager.cs` gerencia a densidade populacional e o ciclo de vida dos veículos NPC ao redor do jogador e da câmera.

---

## 2. Regras de Geração

1. **Limite Rígido de Densidade**:
   - `maxActiveVehicles`: Padrão de 16 veículos simultâneos (escalável entre 8 e 25 conforme perfil de hardware Android).
2. **Área Perimetral de Spawn**:
   - `minSpawnDistance`: 35 metros (fora do campo visual imediato).
   - `maxSpawnDistance`: 85 metros (dentro do alcance de carregamento do setor).
3. **Verificação de Desobstrução**:
   - Antes de posicionar o veículo, executa `Physics.OverlapSphereNonAlloc` com raio de 3m no waypoint candidato. Se houver outro veículo ou obstáculo, o spawn é cancelado.
4. **Distribuição da Frota**:
   - Carros Populares: 40%
   - Motocicletas Civis: 25%
   - Sedãs: 20%
   - Veículos Pesados (Caminhões, Pickups, Ônibus): 15%
