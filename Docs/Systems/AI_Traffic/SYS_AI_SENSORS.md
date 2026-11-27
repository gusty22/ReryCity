# SYS_AI_SENSORS — Sistema Sensorial e Percepção de Obstáculos

---
type: system_specification
id: SYS-13.3
status: IMPLEMENTADO_EM_ETAPA_5_2
created: 2026-11-23
phase: 5
tags:
  - ai_sensors
  - zero_gc
  - raycast_spherecast
  - player_detection
dependencies:
  - SYS_AI_VEHICLES
---

## 1. Princípios de Percepção

O componente `AIVehicleSensor.cs` provê percepção espacial em 360° para tomada de decisões seguras no trânsito.

Para viabilizar execução em dispositivos móveis Android sem engasgos (*frame drops*), o sistema opera sob o princípio de **Zero Alocação no Heap (Zero GC)**:
- Utiliza `Physics.SphereCastNonAlloc` e `Physics.RaycastNonAlloc` com buffers estáticos reutilizáveis (`_raycastBuffer`).
- Filtra colisores próprios em cache local (`_ownColliders`).
- Permite cadência adaptativa controlada pelo `TrafficLODManager`.

---

## 2. Zonas de Detecção

1. **Cone Frontal de Médio/Longo Alcance**:
   - `SphereCast` com raio de 0.8m e alcance configurável de 30m a 55m.
   - Mede distância euclidiana, velocidade relativa e identifica se o alvo é o jogador (`isPlayer`).
2. **Varredura Lateral (Ponto Cego)**:
   - Raios laterais ortogonais de 4.5m para detecção de veículos paralelos.
   - Raios diagonais traseiros de 12.0m para checar espaço livre antes de ingressar na faixa adjacente.
3. **Sensor Traseiro**:
   - Raio traseiro de 6.75m para evitar recuo sobre outros veículos.

---

## 3. Interação Especial com o Jogador (Motocicleta)

Quando o alvo detectado no cone frontal possui a tag `Player` ou componentes da motocicleta (`Titan160`):
- O sensor sinaliza `closestHit.isPlayer = true`.
- Caso o jogador corte a frente da viatura ou freie bruscamente ($d < 3.5m$), a máquina de estados engata instantaneamente o estado `EmergencyStop`, prevenindo atropelamentos e colisões frustrantes.
