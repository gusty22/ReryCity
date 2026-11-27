# Sistema de Integração de Áudio e Espacialização (SYS_AUDIO_INTEGRATION)

---
system_id: SYS_AUDIO_INTEGRATION
parent_system: SYS-08 (Áudio Granular & Roncos)
status: IMPLEMENTADO_EM_FASE_5_6
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_AUDIO]]
  - [[SYS_ENGINE]]
  - [[SYS_ATHLON]]
  - [[SYS_POLICE_PURSUIT]]
---

## 1. Visão Geral

O **Sistema de Integração de Áudio e Espacialização** (`AudioIntegrationManager`) atua como a mesa de mixagem central do Projeto Grau 2, organizando canais de áudio 2D e 3D, volumes mestres e garantindo tratamento defensivo caso arquivos de áudio não estejam presentes no projeto.

---

## 2. Canais de Áudio e Espacialização

* **Motor e Escapamento (3D)**: Gerenciados pelo `MotorAudioController` em sincronia com `EngineController` e `AthlonECUController`.
* **Sirenes Policiais (3D Espacial)**:
  - `spatialBlend = 1.0f`
  - Atenuação logarítmica com `minDistance = 5m` e `maxDistance = 80m`.
  - Efeito Doppler calibrado (`dopplerLevel = 1.2f`) para realismo em passagens de viatura e perseguições.
* **Interface do Usuário (2D Estéreo)**:
  - Sons de toque de botões, buzzer de infração de trânsito e alerta sonoro de ocorrências viárias.
* **Colisão e Impacto (3D Espacial)**:
  - Sons de colisão de lataria, choque com cones e quedas de moto com volume proporcional à energia cinética do impacto.

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `AudioIntegrationManager.cs` | `1000000000000000000000000000009b` | Roteador e mixer de áudio mestre |
| `MotorAudioController.cs` | `10000000000000000000000000000008` | Áudio dinâmico de propulsão e estampidos |
