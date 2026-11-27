---
id: SYS_TRANSMISSION
tipo: sistema
codigo: SYS-03
status: IMPLEMENTADO_EM_FASE_2
responsavel: Engenheiro de Propulsão & Arquiteto de Software
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_ENGINE, TransmissionData, Rigidbody]
arquivos_relacionados: [TransmissionController.cs, TransmissionData.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[MAPEAMENTO_FISICA_MOTOCICLETAS]]
---

# SISTEMA DE TRANSMISSÃO E EMBREAGEM (SYS-03: TRANSMISSION)

## 1. Identificação e Objetivo
* **Código**: `SYS-03`
* **Nome**: Caixa de Câmbio, Embreagem e Relação Final (*Transmission Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_2`.
* **Objetivo**: Transferir o torque mecânico do virabrequim para a roda traseira através das relações de redução primária, marcha selecionada e coroa/pinhão, modulado pela embreagem.

## 2. Equações Mecânicas Implementadas
1. **Relação Total de Redução ($R_{total}$)**:
   $$R_{total} = R_{primaria} \times R_{marcha}(i) \times R_{final}$$
2. **Torque na Roda Traseira ($\tau_{roda}$)**:
   $$\tau_{roda} = \tau_{motor} \times R_{total} \times \eta_{mecanica} \times C_{embreagem}$$
3. **Força Tratora Longitudinal ($F_{tracao}$)**:
   $$F_{tracao} = \frac{\tau_{roda}}{r_{roda}}$$
4. **Queda de Rotação (Gear Drop)**:
   Ao subir de marcha ($i \to i+1$), o RPM do motor cai na proporção exata $\frac{R_{marcha}(i+1)}{R_{marcha}(i)}$.
