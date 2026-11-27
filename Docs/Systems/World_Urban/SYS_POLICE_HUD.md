# SYS_POLICE_HUD — Interface de Perseguição e Nível de Procurado

---
type: system_specification
id: SYS-20.1
status: IMPLEMENTADO_EM_FASE_5_3
created: 2026-11-23
phase: 5
tags:
  - police_hud
  - wanted_level_ui
  - stars_indicator
  - ui
dependencies:
  - SYS_WANTED_LEVEL
  - SYS_POLICE_AI
---

## 1. Visão Geral

Os componentes `WantedLevelUI.cs` e `PoliceHUD.cs` fornecem o feedback visual e de áudio da atividade policial para o jogador no topo e laterais da tela.

---

## 2. Elementos de Apresentação

1. **Estrelas de Procurado (`WantedLevelUI.cs`)**:
   - 5 estrelas exibidas no topo da interface.
   - Ativas em amarelo dourado, inativas em cinza translúcido.
   - Efeito de piscar contínuo durante o período de evasão (quando fora de vista policial).
2. **Banner de Estado Policial (`PoliceHUD.cs`)**:
   - Informa o status da guarnição ("PATRULHAMENTO", "EM PERSEGUIÇÃO!", "TENTATIVA DE INTERCEPTAÇÃO!", "ORDEM DE PARADA / ABORDAGEM!").
3. **Telemetria de Proximidade**:
   - Distância em metros da viatura policial mais próxima da moto do jogador.
4. **Alertas Textuais de Infração**:
   - Popups temporizados de 3.5s indicando a natureza da transgressão e pontuação.
5. **Barra de Abordagem / Prisão (Busted Progress)**:
   - Slider progressivo exibido quando a viatura está dentro do raio de abordagem com moto imobilizada.
