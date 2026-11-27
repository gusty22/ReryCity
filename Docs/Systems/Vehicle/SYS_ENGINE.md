---
id: SYS_ENGINE
tipo: sistema
codigo: SYS-02
status: IMPLEMENTADO_EM_FASE_2
responsavel: Engenheiro de Propulsão & Arquiteto de Software
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_TRANSMISSION, SYS_ATHLON, EngineData, Rigidbody]
arquivos_relacionados: [EngineController.cs, EngineData.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[MAPEAMENTO_FISICA_MOTOCICLETAS]], [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]], [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE MOTOR E DINAMÔMETRO (SYS-02: ENGINE)

## 1. Identificação e Objetivo
* **Código**: `SYS-02`
* **Nome**: Sistema de Motor de Combustão e Dinamômetro (*Engine Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_2`.
* **Objetivo**: Simular a termodinâmica rotacional do virabrequim, curva de torque por RPM, potência em Cavalos-Vapor (CV), inércia do volante, freio-motor e resposta do acelerador.

## 2. Modelagem Física Implementada
1. **Curva de Torque Dinamométrico**:
   $$\tau_{combustao} = Acelerador_{efetivo} \times \text{TorqueCurve}(RPM)$$
2. **Potência no Virabrequim (CV)**:
   $$P_{CV} = \frac{\tau_{Nm} \times RPM}{7023.5}$$
3. **Dinâmica do Volante em Neutro**:
   $$J_{flywheel} \dot{\omega} = \tau_{liquido} = \tau_{combustao} - \tau_{atrito} - \tau_{compressao}$$
4. **Acoplamento à Roda Traseira**:
   Quando a embreagem está acoplada, o RPM converge para a velocidade linear da roda multiplicada pela relação total de transmissão.

## 3. Scripts e Parâmetros Configuráveis
* `EngineData.cs` (`ScriptableObject`): Define marcha lenta (`idleRpm`), teto (`maxRpm`), curva de torque em `AnimationCurve`, inércia (`flywheelInertia`) e velocidade da borboleta.
* `EngineController.cs` (`MonoBehaviour`): Executa em `FixedUpdate` (60 Hz) calculando o RPM e torque líquido.
