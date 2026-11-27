---
id: ADR_003
tipo: adr
status: APROVADO
data: 2026-11-23
sistemas_afetados: [SYS-01, SYS-02, SYS-04, SYS-06, SYS-07, SYS-16]
---

# ADR 003: Parametrização Baseada em ScriptableObjects

## Contexto e Problema
O jogo terá dezenas de motocicletas (CG 160 Titan, Pop 110, XJ6, Hornet, Tiger, etc.), cada uma com massa, centro de massa, ângulo de equilíbrio, torque e curvas de potência completamente distintos. Se esses valores fossem embutidos diretamente nos scripts C#, qualquer balanceamento exigiria recompilação.

## Decisão Adotada
Utilizar `ScriptableObjects` (`MotorcycleData`, `EngineTuningConfig`, `SuspensionPreset`) para armazenar todos os dados e curvas de configuração física. Os controladores em tempo de execução tornam-se cascas lógicas genéricas que recebem esses dados por injeção de dependência.

## Consequências
* **Positivas**: Permite que designers equilibrem as motos em tempo real no Unity Inspector; novos veículos podem ser criados duplicando assets sem escrever uma única linha de código.
* **Negativas**: Exige validação prévia de valores nulos ou intervalos fora dos limites aceitáveis.
