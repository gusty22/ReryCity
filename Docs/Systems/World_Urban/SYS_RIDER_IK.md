---
id: SYS_RIDER_IK
tipo: sistema
codigo: SYS-11-IK
status: IMPLEMENTADO_EM_FASE_4
responsavel: Engenheiro de Animação & Cinemática
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_RIDER, Animator]
arquivos_relacionados: [RiderIKController.cs]
referencia_tecnica: [[SYS_RIDER]]
---

# SISTEMA DE CINEMÁTICA INVERSA DO PILOTO (SYS-11-IK)

## 1. Identificação e Objetivo
* **Código**: `SYS-11-IK`
* **Nome**: Cinemática Inversa do Piloto (*Rider IK Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_4`.
* **Objetivo**: Fixar as mãos do piloto nas manoplas do guidão e os pés nas pedaleiras da moto em tempo real, independente de vibrações, curvas ou alterações de geometria do guidão na oficina.

## 2. Metodologia de Implementação
* Utiliza o método nativo `OnAnimatorIK` do Unity Animator.
* Executa com zero alocação de memória no heap (`0 KB GC alloc`), ideal para dispositivos móveis Android.
* Metas controladas:
  * `AvatarIKGoal.LeftHand` e `AvatarIKGoal.RightHand` (peso $1.0$).
  * `AvatarIKGoal.LeftFoot` e `AvatarIKGoal.RightFoot` (peso $1.0$).
  * `SetLookAtPosition` com foco dinâmico no ponto de fuga da curva.
