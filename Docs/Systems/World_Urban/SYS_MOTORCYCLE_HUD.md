# Sistema de Painel e Instrumentos da Motocicleta (SYS_MOTORCYCLE_HUD)

---
system_id: SYS_MOTORCYCLE_HUD
parent_system: SYS-20 (Interface e HUD)
status: IMPLEMENTADO_EM_FASE_5_6
date: 2026-11-23
author: AI Senior Systems Architect
dependencies:
  - [[SYS_WHEELIE]]
  - [[SYS_ENGINE]]
  - [[SYS_TRANSMISSION]]
  - [[SYS_ATHLON]]
  - [[SYS_CAMERA]]
---

## 1. Visão Geral

O **Sistema de Painel e Instrumentos da Motocicleta** (`MotorcycleHUD`) é o componente visual responsável por apresentar a telemetria física real da motocicleta em tempo real na tela do jogador, sem interpolações artificiais ou números fictícios.

O HUD extrai os parâmetros diretamente dos sistemas de física e propulsão:
* **Velocidade**: Velocímetro digital em km/h extraído do vetor de velocidade do `Rigidbody`.
* **Tacômetro (RPM)**: Rotação real por minuto do motor (`EngineController`), com barra gráfica parametrizada até a faixa de rotação máxima (`maxRpm`).
* **Indicador de Marcha**: Exibição da marcha selecionada no câmbio sequencial (`TransmissionController`), renderizando "N" para neutro e "1" a "5" para as marchas engatadas.
* **Corte de Giro (Rev Limiter)**: Indicador visual estroboscópico ativado quando a rotação atinge o limitador mecânico da ECU Athlon (`IsCutActive`).
* **Two-Step**: Indicador luminoso de aquecimento/largada com mapa de RPM travado para arrancada (`IsTwoStepActive`).
* **Ângulo de Grau**: Telemetria angular da inclinação longitudinal (`CurrentPitchAngle`), sinalizando empinadas com badge específico quando a roda dianteira perde contato com o asfalto.
* **Modo de Câmera**: Exibição da perspectiva ativa da câmera Cinemachine (`ThirdPersonClose`, `ThirdPersonFar`, `FirstPerson`, `Hood`, `DynamicWheelie`).

---

## 2. Princípios de Otimização Mobile (Zero GC)

1. **Dirty Checking com Inteiros em Cache**:
   - As variáveis numéricas de velocidade, RPM, marcha e ângulo são armazenadas em inteiros cacheados (`_cachedSpeed`, `_cachedRpm`, etc.).
   - A conversão de strings (`ToString()`) e a atribuição aos componentes `Text` ocorrem exclusivamente quando o valor inteiro se altera, eliminando a geração de lixo no heap gerenciado a cada frame.
2. **Atualização Throttled (20 Hz)**:
   - A rotina de varredura roda em cadência otimizada (`updateInterval = 0.05s`), aliviando ciclos de renderização de texto da CPU em chips móveis ARM64.

---

## 3. Matriz de Componentes

| Componente | GUID Determinístico | Responsabilidade |
| :--- | :--- | :--- |
| `MotorcycleHUD.cs` | `10000000000000000000000000000098` | Painel de instrumentos e telemetria da moto |
| `WorldEventNotificationUI.cs` | `10000000000000000000000000000099` | Feed de notificações de acidentes, obras e blitz |
| `UrbanTrafficTestScene.unity` | N/A | Canvas com `CanvasScaler` adaptativo (1920x1080) |
