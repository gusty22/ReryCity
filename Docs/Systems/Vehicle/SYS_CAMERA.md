---
id: SYS_CAMERA
tipo: sistema
codigo: SYS-09
status: IMPLEMENTADO_EM_FASE_3
responsavel: Engenheiro de Gameplay & Especialista em Visão
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_MOTORCYCLE, SYS_WHEELIE, Rigidbody]
arquivos_relacionados: [MotorcycleCameraController.cs]
referencia_tecnica: [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]]
---

# SISTEMA DE CÂMERAS MULTIPLATAFORMA (SYS-09: CAMERA SYSTEM)

## 1. Identificação e Objetivo
* **Código**: `SYS-09`
* **Nome**: Sistema de Câmeras Dinâmicas Multiplataforma (*Motorcycle Camera Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_3`.
* **Objetivo**: Prover perspectivas imersivas e dinâmicas de pilotagem para PC e Android: terceira pessoa orbital anti-clipping, primeira pessoa no capacete com inclinação de curva (*lean banking*) e câmera de ação dramática para manobras de grau.

## 2. Modos de Câmera Implementados
1. **ThirdPersonChase (3ª Pessoa Clássica)**:
   * Acompanhamento amortecido da posição e yaw da moto.
   * Órbita livre horizontal/vertical via mouse ou touch com auto-recentralização após 1.5s de inatividade.
   * Sistema anti-clipping via `SphereCast` evitando que a câmera atravesse paredes e postes do cenário.
2. **FirstPersonHelmet (1ª Pessoa no Capacete)**:
   * Ancorada na posição da cabeça do motociclista (`firstPersonOffset`).
   * Inclinação dinâmica nas curvas (*lean banking* a $65\%$), simulando o piloto compensando a força centrífuga com o tronco.
   * FOV aumentado para $75^\circ$, garantindo ampla visão do guidão, manoplas e painel.
3. **WheelieStuntAction (Câmera de Ação do Grau)**:
   * Enquadramento angular diagonal rebaixado, focalizando a ascensão da roda dianteira e a postura da moto sem trepidações.
