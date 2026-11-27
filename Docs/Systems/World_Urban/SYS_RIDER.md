---
id: SYS_RIDER
tipo: sistema
codigo: SYS-11
status: IMPLEMENTADO_EM_FASE_4
responsavel: Engenheiro de Gameplay & Especialista em Animação
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_MOTORCYCLE, SYS_WHEELIE, RiderData]
arquivos_relacionados: [MotorcycleRiderController.cs, RiderData.cs]
referencia_tecnica: [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]], [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DO PILOTO HUMANOIDE (SYS-11: RIDER CONTROLLER)

## 1. Identificação e Objetivo
* **Código**: `SYS-11`
* **Nome**: Sistema do Piloto Humanoide (*Motorcycle Rider Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_4`.
* **Objetivo**: Controlar o personagem humanoide que pilota a motocicleta, gerenciando estados de pilotagem (Parado, Aceleração, Frenagem, Curvas, Grau e Raspão), orquestrando o IK, a postura corporal e disparando a transição para Ragdoll físico em caso de queda.

## 2. Estados Operacionais do Piloto
1. `Idle`: Parado sobre o assento com motor em marcha lenta.
2. `Riding`: Pilotagem de cruzeiro acompanhando curvas e velocidade da moto.
3. `Accelerating`: Tronco abaixado contra o tanque para reduzir arrasto.
4. `Braking`: Corpo empurrado para trás contra o guidão sob desaceleração.
5. `WheelieAscending`: Tronco recuando conforme a roda dianteira sobe.
6. `WheelieBalanced`: Postura ereta e relaxada no ponto de equilíbrio ($55^\circ \text{ a } 65^\circ$).
7. `WheelieRaspa`: Postura esticada para trás com ângulo extremo ($\ge 72^\circ$).
8. `CrashedRagdoll`: Ejeção física e desativação do Animator.
