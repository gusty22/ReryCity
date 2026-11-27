---
id: SYS_CUSTOMIZATION
tipo: sistema
codigo: SYS-16
status: IMPLEMENTADO_EM_FASE_3
responsavel: Engenheiro Físico & Especialista em Customização
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_WORKSHOP, SYS_ENGINE, SYS_TRANSMISSION, SYS_SUSPENSION]
arquivos_relacionados: [WorkshopManager.cs, PartItemData.cs, SuspensionController.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE CUSTOMIZAÇÃO MECÂNICA E ESTÉTICA (SYS-16: CUSTOMIZATION)

## 1. Identificação e Objetivo
* **Código**: `SYS-16`
* **Nome**: Customização Mecânica e Estética (*Mechanical & Visual Tuning*).
* **Status**: `IMPLEMENTADO_EM_FASE_3`.
* **Objetivo**: Garantir que as peças instaladas na oficina modifiquem fisicamente o comportamento dinâmico da moto (massa, torque, curva de potência, relação de transmissão, rigidez de suspensão, frenagem e aderência de pneus), e não apenas seu aspecto cosmético.

## 2. Impactos Físicos Reais por Componente
1. **Motor**: Substitui o `EngineData` no `EngineController`, alterando a curva dinamométrica e o RPM máximo.
2. **Escapamento**: Conecta novo `MotorAudioProfile` ao `MotorAudioController`, alterando o pitch, cadência de estouros e ronco.
3. **Coroa e Pinhão**: Altera `finalDriveRatio` no `TransmissionController`. Relação curta (ex: 48/14 = 3.428) amplifica a força tratora e a facilidade de puxar o grau em 2ª e 3ª marcha; relação longa (ex: 38/16 = 2.375) aumenta a velocidade máxima em reta.
4. **Suspensão (Molejo)**: O `SuspensionController` aplica equações $F = k \cdot x + c \cdot v$. Molas duras evitam afundamento excessivo na aterrissagem; molas macias absorvem irregularidades da pista.
