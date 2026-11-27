# SYS_AI_DRIVER — Perfis Comportamentais e Máquina de Estados da IA

---
type: system_specification
id: SYS-13.2
status: IMPLEMENTADO_EM_ETAPA_5_2
created: 2026-11-23
phase: 5
tags:
  - ai_driver
  - state_machine
  - driver_profiles
  - overtaking
dependencies:
  - SYS_AI_VEHICLES
  - SYS_TRAFFIC_NAVIGATION
---

## 1. Perfis de Motoristas (`AIDriverProfile.cs`)

O sistema modela 3 perfis psicológicos de condutores civis:

### 1.1. Cauteloso (`DriverProfile_Cautious.asset`)
* Multiplicador de distância segura: **1.4x** (parada antecipada).
* Margem da linha de retenção: **3.5m**.
* Aceleração: **0.8x** (suave).
* Tolerância de velocidade: **-5 km/h** abaixo da via.
* Propensão a ultrapassagem: **15%** (apenas com ampla visibilidade).
* Conformidade semafórica: **100%**.

### 1.2. Normal (`DriverProfile_Normal.asset`)
* Multiplicador de distância segura: **1.0x** (padrão urbano).
* Margem da linha de retenção: **2.5m**.
* Aceleração: **1.0x** (progressiva).
* Tolerância de velocidade: **0 km/h**.
* Propensão a ultrapassagem: **35%**.
* Conformidade semafórica: **100%**.

### 1.3. Agressivo (`DriverProfile_Aggressive.asset`)
* Multiplicador de distância segura: **0.75x** (seguimento próximo).
* Margem da linha de retenção: **1.8m**.
* Aceleração: **1.25x** (arranque rápido).
* Tolerância de velocidade: **+8 km/h** acima da via.
* Propensão a ultrapassagem: **70%** (frequente quando a faixa esquerda estiver livre).
* Conformidade semafórica: **98%** (arranque agressivo no amarelo/verde).

---

## 2. Máquina de Estados Finita (`AIVehicleStateMachine.cs`)

Composta por 10 estados finitos com histerese temporal configurável (padrão 0.35s):

1. `Idle`: Veículo desativado ou aguardando no pool.
2. `Cruise`: Velocidade regulamentada seguindo os waypoints da faixa.
3. `FollowVehicle`: Seguindo veículo à frente adaptando o acelerador.
4. `Brake`: Redução gradual de velocidade diante de curva ou aproximação.
5. `StopAtSignal`: Imobilizado na linha de retenção diante de semáforo vermelho.
6. `IntersectionWaiting`: Aguardando desobstrução de cruzamento prioritário ou rotatória.
7. `LaneChanging`: Manobra de transição lateral suave para faixa vizinha.
8. `Overtaking`: Ultrapassagem ativa acelerando na faixa esquerda.
9. `AvoidObstacle`: Desvio de obstáculo fixo ou veículo avariado.
10. `EmergencyStop`: Frenagem total máxima disparada por perigo iminente (fura a histerese).
