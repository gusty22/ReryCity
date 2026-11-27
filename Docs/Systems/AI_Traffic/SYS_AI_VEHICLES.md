# SYS_AI_VEHICLES — Veículos Civis NPC e Física Veicular

---
type: system_specification
id: SYS-13.1
status: IMPLEMENTADO_EM_ETAPA_5_2
created: 2026-11-23
phase: 5
tags:
  - npc
  - ai
  - vehicle_physics
  - mobile_60fps
dependencies:
  - SYS_TRAFFIC_NAVIGATION
  - ADR_001_UNITY6_URP
  - ADR_002_CLEAN_ARCHITECTURE_PHYSICS
  - ADR_003_SCRIPTABLE_OBJECTS_DATA
  - ADR_004_CLEAN_ROOM_RECONSTRUCTION
---

## 1. Visão Geral do Sistema

O subsistema de **Veículos Civis NPC** provê tráfego urbano vivo, fisicamente consistente e performático para o *Projeto Grau 2*. 

Projetado especificamente para rodar a 60 FPS no Android, o sistema não utiliza movimentação cinemática arbitrária (`transform.Translate`), mas sim equações newtonianas aplicadas ao `Rigidbody` em `FixedUpdate`, garantindo interações de colisão realistas com o jogador e outros veículos.

---

## 2. Categorias e Dados Parametrizados (`AIVehicleData.cs`)

O tráfego urbano é composto por 8 categorias distintas de veículos:

1. **CivilMotorcycle**: Motocicleta leve (140 kg), ágil, com inclinação visual (*lean banking*) de até 28° em curvas.
2. **EconomyCar**: Carro popular hatch (980 kg), tração moderada, ampla presença na malha urbana.
3. **Sedan**: Sedã médio (1350 kg), cruzeiro estável em avenidas.
4. **SUV**: Utilitário esportivo (1600 kg), maior altura de sensor e centro de gravidade.
5. **Pickup**: Caminhonete (2100 kg), aceleração progressiva e frenagem prolongada.
6. **Truck**: Caminhão urbano de entregas (7500 kg), raio de giro amplo, distância de segurança de 12 metros.
7. **Bus**: Ônibus coletivo (11500 kg), transporte coletivo, grande porte e baixa agilidade.
8. **PoliceCruiser**: Viatura policial de patrulha (1550 kg), motor potente (preparação para Etapa 5.4).

---

## 3. Arquitetura Modular de Controladores

* `AIVehicleController.cs`: Orquestrador central que integra sensores, máquina de estados e navegação vetorial.
* `AIVehicleMotorController.cs`: Propulsão longitudinal por forças no Rigidbody, clamping de velocidade e marcha a ré.
* `AIVehicleSteeringController.cs`: Algoritmo Pure Pursuit/Stanley para seguimento de faixas, atenuação em alta velocidade e inclinação visual para motos.
* `AIVehicleBrakeController.cs`: Frenagem desacelerativa física e trava anti-recuo em repouso.
* `AIVehicleStateMachine.cs`: Máquina de estados com histerese mínima para evitar oscilações estocásticas.
* `AIVehicleSensor.cs`: Varredura sensorial frontal e lateral não-alocativa (`SphereCastNonAlloc`, `RaycastNonAlloc`).
