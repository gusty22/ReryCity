---
id: SYS_ATHLON
tipo: sistema
codigo: SYS-06
status: IMPLEMENTADO_EM_FASE_2
responsavel: Engenheiro Eletrônico & Arquiteto de Software
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_ENGINE, SYS_AUDIO, AthlonECUData]
arquivos_relacionados: [AthlonECUController.cs, AthlonECUData.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[ANALISE_FISICA_MOTOS]]
---

# SISTEMA DE INJEÇÃO PROGRAMÁVEL ATHLON (SYS-06: ATHLON ECU)

## 1. Identificação e Objetivo
* **Código**: `SYS-06`
* **Nome**: Módulo de Injeção Eletrônica Programável Athlon (*Athlon ECU Controller*).
* **Status**: `IMPLEMENTADO_EM_FASE_2`.
* **Objetivo**: Controlar o limitador principal de RPM, controle de largada (Two-Step), corte de ignição/injeção por histerese (Schmitt Trigger) e disparo de estampidos sonoros/visuais no escapamento.

## 2. Lógica e Fórmulas de Corte
1. **Comparador com Histerese**:
   * Se $RPM \ge RPM_{corte} \implies \text{Corte Ativo}$.
   * Se $RPM \le (RPM_{corte} - \text{Histerese}) \implies \text{Corte Desativado}$.
2. **Two-Step de Largada**:
   * Ativado quando a velocidade for $< 4\text{ km/h}$ e acelerador $> 75\%$.
   * Limita o motor na rotação teto `twoStepRpm` (ex: $5200\text{ RPM}$) para arrancadas controladas sem empinar involuntariamente.
3. **Pops & Bangs (Estampidos)**:
   * Sob corte de ignição, emite eventos estocásticos `OnBackfireTriggered` consumidos pelo sistema de áudio e partículas.
