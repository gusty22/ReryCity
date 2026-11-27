---
id: SYS_RIDER_ANIMATIONS
tipo: sistema
codigo: SYS-11-ANIM
status: IMPLEMENTADO_EM_FASE_4
responsavel: Animador Técnico & Engenheiro de Gameplay
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_RIDER, SYS_WHEELIE, RiderData]
arquivos_relacionados: [RiderPoseController.cs]
referencia_tecnica: [[SYS_RIDER]]
---

# POSTURA DINÂMICA E ANIMAÇÕES DE GRAU (SYS-11-ANIM)

## 1. Identificação e Objetivo
* **Código**: `SYS-11-ANIM`
* **Nome**: Postura Dinâmica e Animações de Manobras (*Rider Pose Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_4`.
* **Objetivo**: Modular proceduralmente em `LateUpdate` os ossos da coluna (`Spine`), tórax (`Chest`) e cabeça (`Head`), gerando respostas naturais à aceleração, frenagem, curvas e empinadas.

## 2. Parâmetros Físicos Integrados
* **Inclinação Lateral de Curva (*Lean Banking*)**:
  Tronco inclina na proporção `leanInCurveFactor` ($0.85$), simulando o piloto deitando a moto e apoiando o joelho.
* **Projeção de Arfagem no Grau**:
  Conforme o ângulo de arfagem $\theta$ aumenta, o tronco recua até `wheelieSpineBackLeanAngle` ($28^\circ$).
* **Postura de Raspão ($\theta \ge 70^\circ$)**:
  Adiciona $-15^\circ$ extras no tronco, alinhando a cabeça e as costas com o para-lama traseiro.
