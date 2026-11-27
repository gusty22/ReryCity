---
id: SYS_WHEELIE
tipo: sistema
codigo: SYS-01
status: IMPLEMENTADO_EM_FASE_1
responsavel: Arquiteto de Software & Engenheiro Físico
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS-02, SYS-04, SYS-05, SYS-07, Rigidbody, MotorcycleData, IMotorcycleInput]
arquivos_relacionados: [WheeliePhysicsController.cs, MotorcycleController.cs, MotorcycleData.cs, IMotorcycleInput.cs, PhysicsTelemetryUI.cs, CameraFollowPhysics.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[MAPEAMENTO_FISICA_MOTOCICLETAS]], [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]], [[ADR_003_SCRIPTABLE_OBJECTS_DATA]], [[ADR_004_CLEAN_ROOM_RECONSTRUCTION]]
---

# SISTEMA DE MANOBRAS E FÍSICA DO GRAU (SYS-01: WHEELIE)

## 1. Identificação e Objetivo
* **Código**: `SYS-01`
* **Nome**: Sistema de Manobras e Física do Grau (*Wheelie Physics Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_1` (Unity 6 / C#).
* **Objetivo**: Controlar as forças físicas e torques rotacionais no eixo de arfagem (*pitch*) que permitem ao jogador empinar a moto, encontrar o ponto de equilíbrio dinâmico, controlar a altura com o acelerador e freio a pé, executar manobras posturais e evitar o capotamento.

## 2. Requisitos e Regras Físicas Implementadas
* **REQ-WHL-01**: Implementado torque de elevação $\tau_{lift}$ com amortecimento suave próximo a `balanceAngle` (faixa `liftEaseRange`).
* **REQ-WHL-02**: Implementado torque restaurador da gravidade $\tau_{gravity} = m \cdot g \cdot L_{cm} \cdot \cos(\theta)$, que se anula no alinhamento vertical com a roda traseira.
* **REQ-WHL-03**: Freio a pé traseiro ([[SYS_BRAKES]]) aplicando contra-torque instantâneo ($\tau_{brake} = B \cdot \tau_{max\_brake}$) para baixar a dianteira quando $\theta > 5^\circ$.
* **REQ-WHL-04**: Redução e bloqueio do esterçamento direcional do guidão quando $\theta > \text{noTurnAngle}$ ($52^\circ$ nominal).
* **REQ-WHL-05**: Detecção de capotamento em $\theta \ge \text{crashAngle}$ ($83^\circ$) com congelamento de comandos físicos e notificação via evento `OnCrashTriggered`.
* **REQ-WHL-06**: Cronometragem e evento de telemetria física a 60 Hz em `FixedUpdate`.

## 3. Arquitetura de Scripts (Clean Room)
1. `MotorcycleData.cs`: ScriptableObject com todos os parâmetros nominais de massa, inércia, torques, limiares angulares e assistências.
2. `IMotorcycleInput.cs`: Contrato de abstração universal desacoplando teclado, gamepad, touch mobile e IA.
3. `CrossPlatformInputHandler.cs`: Implementação multiplataforma PC (Teclado) e Mobile (UI Touch).
4. `WheeliePhysicsController.cs`: Núcleo matemático de cálculo diferencial de torque angular no eixo local X.
5. `MotorcycleController.cs`: Dinâmica longitudinal, tração da roda traseira, esterçamento condicional e downforce.
6. `PhysicsTelemetryUI.cs`: Monitoramento e telemetria em tempo real (Pitch, Velocidade Angular, Torques, Estado de Grau, Alerta de Queda).
7. `CameraFollowPhysics.cs`: Câmera de terceira pessoa suave e isolada de vibrações extremas de arfagem.

## 4. Cena de Validação e Ativos
* **Cena**: `Assets/Scenes/PhysicsTestScene.unity`
* **Asset de Calibração**: `Assets/ScriptableObjects/Motorcycles/Titan160_Data.asset` (massa 125 kg, ângulo de equilíbrio $60^\circ$, torque de subida 420 Nm, freio 550 Nm).
