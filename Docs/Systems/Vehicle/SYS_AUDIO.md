---
id: SYS_AUDIO
tipo: sistema
codigo: SYS-08
status: IMPLEMENTADO_EM_FASE_2
responsavel: Engenheiro de Áudio & Arquiteto de Software
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_ENGINE, SYS_TRANSMISSION, SYS_ATHLON, MotorAudioProfile]
arquivos_relacionados: [MotorAudioController.cs, MotorAudioProfile.cs]
referencia_tecnica: [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]], [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE ÁUDIO DINÂMICO E RONCOS (SYS-08: MOTOR AUDIO)

## 1. Identificação e Objetivo
* **Código**: `SYS-08`
* **Nome**: Sistema de Áudio Dinâmico de Motor e Escapamento (*Motor Audio Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_2`.
* **Objetivo**: Renderizar o som acústico do motor de forma dinâmica e contínua, modulando pitch e volume em tempo real a partir do RPM, carga do acelerador, trocas de marcha e estampidos da injeção Athlon.

## 2. Arquitetura Desacoplada (Clean Architecture)
* O `MotorAudioController` atua estritamente como **observador** (padrão Observer).
* Não realiza cálculos de força, torque ou aceleração física.
* Modula pitch proporcionalmente entre `minPitch` e `maxPitch`.
* Dispara sons pontuais (`PlayOneShot`) com variações aleatórias de pitch para estampidos e trocas de marchas.
