---
id: SYS_RIDER_RAGDOLL
tipo: sistema
codigo: SYS-11-RAGDOLL
status: IMPLEMENTADO_EM_FASE_4
responsavel: Engenheiro de Física & Simulação
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_RIDER, Rigidbody, CharacterJoint]
arquivos_relacionados: [RiderRagdollController.cs]
referencia_tecnica: [[SYS_RIDER]], [[SYS_WHEELIE]]
---

# SISTEMA DE RAGDOLL E QUEDAS FÍSICAS (SYS-11-RAGDOLL)

## 1. Identificação e Objetivo
* **Código**: `SYS-11-RAGDOLL`
* **Nome**: Sistema de Ragdoll e Ejeção em Quedas (*Rider Ragdoll Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_4`.
* **Objetivo**: Controlar a transição suave e física entre o esqueleto animado e múltiplos corpos rígidos interconectados por juntas (*joints*), acionado quando a moto capota ($\theta \ge \text{crashAngle}$) ou sofre colisão forte.

## 2. Mecânica de Ejeção e Recuperação
1. **Transição para Ragdoll**:
   * Desativa `Animator` e `RiderIKController`.
   * Desacopla o piloto do chassi (`transform.SetParent(null)`).
   * Habilita a simulação física em todos os corpos (`isKinematic = false`, `detectCollisions = true`).
   * Transfere o momento linear da moto somado a um impulso vertical de ejeção ($2.5\text{ m/s}$) e torque de rolamento caótico.
2. **Recuperação e Reset**:
   * Ao pressionar a tecla `R` ou botão virtual de reinicialização, o piloto é re-ancorado na motocicleta, os ossos retornam à postura inicial e o Animator é reativado.
