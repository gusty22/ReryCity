# Sistema de Controles Táteis Mobile (SYS_MOBILE_CONTROLS)

---
system_id: SYS_MOBILE_CONTROLS
parent_system: SYS-10 (Entradas / Input System)
status: IMPLEMENTADO_EM_FASE_5_6
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_INPUT]]
  - [[SYS_BRAKES]]
  - [[SYS_WHEELIE]]
  - [[SYS_ATHLON_MOBILE]]
---

## 1. Visão Geral

O **Sistema de Controles Táteis Mobile** (`TouchscreenMobileControlsUI`) fornece uma interface de pilotagem intuitiva e responsiva para telas touch de smartphones Android, conectando botões e manetes virtuais diretamente ao `CrossPlatformInputHandler`.

---

## 2. Layout Ergonômico dos Controles

* **Direção Lateral (Esquerda da Tela)**:
  - Botões dedicados de esterçamento (`SteerLeft`, `SteerRight`) ou stick virtual analógico com zona morta ajustável.
* **Aceleração e Frenagem (Direita da Tela)**:
  - Acelerador virtual de resposta contínua (`Throttle`).
  - Freio dianteiro independente (`FrontBrake`).
  - Freio traseiro independente (`RearBrake`) para controle de raspagem da placa e equilíbrio no grau.
* **Manobras e Câmbio**:
  - Manete de empinada (`WheelieHold`) para puxada de frente.
  - Botões sequenciais de subida e descida de marcha (`ShiftUp`, `ShiftDown`).
* **Auxiliares**:
  - Alternância de câmera (`CameraCycle`).
  - Reposicionamento pós-queda (`Reset`).
  - Acesso rápido ao celular virtual (`AthlonMobileApp`).

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `TouchscreenMobileControlsUI.cs` | `1000000000000000000000000000009a` | Controlador dos botões virtuais de tela |
| `CrossPlatformInputHandler.cs` | `10000000000000000000000000000010` | Hub multiplataforma PC / Android |
