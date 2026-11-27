---
id: SYS_BRAKES
tipo: sistema
codigo: SYS-05
status: ESPECIFICADO
responsavel: Arquiteto de Software
data_criacao: 2026-11-23
dependencias: [SYS-01, SYS-04, SYS-07, Rigidbody]
arquivos_relacionados: [BrakeController.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[MAPEAMENTO_FISICA_MOTOCICLETAS]]
---

# SISTEMA DE FREIOS COMBINADOS (SYS-05: BRAKES)

## 1. Identificação e Objetivo
* **Código**: `SYS-05`
* **Nome**: Sistema de Freios Duplos (Manete Dianteiro e Pedal Traseiro).
* **Objetivo**: Prover desaceleração física independente para a roda dianteira e traseira da motocicleta, permitindo controle simultâneo de parada em alta velocidade, travamento para derrapagem/drift e estabilização de atitude durante manobras de grau.

## 2. Requisitos e Regras de Negócio
* **REQ-BRK-01**: Separação estrita entre o comando de mão (freio dianteiro) e o comando de pé (freio traseiro).
* **REQ-BRK-02**: O freio dianteiro atua com 70% a 80% do poder de frenagem longitudinal e comprime o garfo dianteiro ([[SYS_SUSPENSION]]). Em velocidade moderada com toque excessivo, pode induzir *stoppie* (RL).
* **REQ-BRK-03**: O freio traseiro atua diretamente sobre o controlador de empinar ([[SYS_WHEELIE]]), fornecendo o contra-torque estabilizador quando o piloto está no grau.
* **REQ-BRK-04**: Suporte a travamento de roda para manobras de derrapagem (cavalo de pau / *drift*).

## 3. Módulos Dependentes e Relacionamentos
* **Alimentado por**: [[SYS_INPUT]] (gatilhos do gamepad ou botões táteis na tela).
* **Módulos Afetados por Alterações no Freio**:
  * [[SYS_WHEELIE]]: Qualquer alteração no tempo de resposta ou curva de torque do freio traseiro altera radicalmente a facilidade de controlar o grau.
  * [[SYS_SUSPENSION]]: Frenagens intensas geram transferência de peso que comprimem o garfo dianteiro ou distendem a balança traseira.
  * [[SYS_MOTORCYCLE]]: Afeta a distância de parada total do veículo e a velocidade do chassi.
