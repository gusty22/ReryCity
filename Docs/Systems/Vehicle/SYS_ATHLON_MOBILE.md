---
id: SYS_ATHLON_MOBILE
tipo: sistema
codigo: SYS-18-ATHLON
status: IMPLEMENTADO_EM_FASE_3
responsavel: Engenheiro de UI & Eletrônica Embarcada
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_ATHLON, SYS_INPUT]
arquivos_relacionados: [AthlonMobileApp.cs, AthlonECUController.cs, AthlonECUData.cs]
referencia_tecnica: [[FORMULAS_FISICA_MOTOS]], [[SYS_ATHLON]]
---

# INTERFACE DA ATHLON NO CELULAR VIRTUAL (SYS-18-ATHLON)

## 1. Identificação e Objetivo
* **Código**: `SYS-18-ATHLON`
* **Nome**: Aplicativo Virtual Athlon Smart ECU (*Athlon Mobile App*).
* **Status**: `IMPLEMENTADO_EM_FASE_3`.
* **Objetivo**: Prover interface funcional simulada na tela de um smartphone in-game, conectada via "Bluetooth" ao `AthlonECUController`, permitindo ao jogador reprogramar limitadores de giro, controle de largada Two-Step, histerese de centelha e perfis de mapa em tempo real.

## 2. Funcionalidades da Interface
* **Ajuste Fino de Hard Cut**: Incremento/decremento de 100 em 100 RPM ($7.000$ a $12.500\text{ RPM}$).
* **Two-Step Selector**: Liga/desliga o limitador secundário de largada e define a rotação de retenção.
* **Seletor de Histerese**: Alterna entre cadência de corte Lenta ($350\text{ RPM}$), Média ($200\text{ RPM}$) e Rápida ($100\text{ RPM}$).
* **Modo de Corte**: Ignição (tiros de escape), Injeção (corte suave sem tiros) e Ambos (corte seco).
* **Gerenciador de Mapas Pré-definidos**: Alternância rápida entre perfis "Original Seguro", "Manobras & Grau" e "Metralhadora Seca".
