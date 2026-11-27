---
id: SYS_SUSPENSION
tipo: sistema
codigo: SYS-04
status: ESPECIFICADO
responsavel: Arquiteto de Software
data_criacao: 2026-11-23
dependencias: [SYS-07, Rigidbody, WheelCollider]
arquivos_relacionados: [SuspensionController.cs, SuspensionPreset.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[MAPEAMENTO_FISICA_MOTOCICLETAS]]
---

# SISTEMA DE SUSPENSÃO E GEOMETRIA (SYS-04: SUSPENSION / MOLEJO)

## 1. Identificação e Objetivo
* **Código**: `SYS-04`
* **Nome**: Sistema de Suspensão e Geometria da Motocicleta.
* **Objetivo**: Simular a dinâmica de molas e amortecedores dianteiros e traseiros, absorvendo imperfeições da pista, modulando a transferência de peso em acelerações e frenagens, e influenciando diretamente o comportamento do grau e aterrissagem da roda dianteira.

## 2. Requisitos e Regras Físicas
* **REQ-SUSP-01**: Suportar ajustes independentes de constante elástica da mola ($k$) e amortecimento viscoso ($c$) para garfo dianteiro e balança traseira.
* **REQ-SUSP-02**: Permitir a regulagem de pré-carga e altura livre do solo (estilos "rebaixada" ou "traseira alta").
* **REQ-SUSP-03**: Prover amortecimento de retorno (*rebound*) suave para evitar quiques descontrolados após a roda dianteira retornar ao solo após o grau.
* **REQ-SUSP-04**: Rigidez traseira calibrável via interface da oficina (`SYS-15: Oficina`), afetando a facilidade de puxar o grau (`SYS-01: Wheelie`).

## 3. Classes e Arquitetura Planejada
* `SuspensionController.cs`: Componente MonoBehaviour anexado ao GameObject da moto. Executa em `FixedUpdate` calculando a força da suspensão em cada roda.
* `SuspensionPreset.cs`: `ScriptableObject` com os parâmetros nominais de curso, mola e amortecedor para cada modelo de moto.

## 4. Relações e Dependências Cruzadas
* **Alimentado por**: `MotorcycleData` (`ScriptableObject`) e `Rigidbody` da moto.
* **Impacta diretamente**:
  * [[SYS_WHEELIE]] (Física do Grau): Suspensão traseira mole absorve parte do torque de subida; suspensão rígida transfere torque imediatamente para a arfagem.
  * [[SYS_BRAKES]] (Frenagem): A frenagem dianteira comprime o garfo dianteiro (mergulho / *dive*).
  * [[SYS_CUSTOMIZATION]] (Customização): A oficina altera as molas e altura livre.

## 5. Fórmulas e Modelagem
A equação de força da suspensão implementada segue o modelo padrão massa-mola-amortecedor detalhado em [[FORMULAS_FISICA_MOTOS#4-dinâmica-da-suspensão-dianteira-e-traseira-molejo]]:
$$F_{suspensao} = k_{mola} \times (x_{repouso} - x) - c_{amortecedor} \times \dot{x}$$
