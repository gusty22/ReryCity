---
id: SYSTEM_ARCHITECTURE
tipo: arquitetura
projeto: Projeto Grau 2
status: ESPECIFICADO
---

# ARQUITETURA GLOBAL DO SISTEMA (SYSTEM ARCHITECTURE)

## 1. Visão em Camadas (Layered Architecture)

A nova implementação do Projeto Grau 2 adota o modelo de quatro camadas desacopladas:

```mermaid
graph TD
    subgraph L1 ["1. Camada de Apresentação & Entrada (UI & Input)"]
        UI["HUD, Menus, Velocímetro, Celular"]
        Input["Unity Input System (Touch / Gamepad)"]
    end

    subgraph L2 ["2. Camada de Domínio e Controle (Gameplay Entities)"]
        MotoController["MotorcycleController (Orquestrador)"]
        Wheelie["WheeliePhysicsController (Física do Grau)"]
        Engine["MotorcycleEngine & EngineTuningController"]
        Brakes["BrakeController & SuspensionController"]
        Police["PoliceAIController & WantedCoordinator"]
    end

    subgraph L3 ["3. Camada de Dados & Configurações (ScriptableObjects)"]
        MotoData["MotorcycleData"]
        TuningData["TuningPreset (Athlon / Injeção)"]
        EconomyData["EconomyLedger & ItemCatalog"]
    end

    subgraph L4 ["4. Camada de Infraestrutura e Persistência"]
        PhysicsEngine["Unity PhysX / Rigidbody 3D Engine"]
        AudioMixer["AudioMixer & Granular Audio System"]
        SaveService["Local Encrypted Save Service"]
    end

    Input --> MotoController
    UI -.->|"Lê Eventos de"| MotoController
    MotoController --> Wheelie
    MotoController --> Engine
    MotoController --> Brakes
    Wheelie --> PhysicsEngine
    Brakes --> PhysicsEngine
    Engine --> AudioMixer
    MotoData --> MotoController
    TuningData --> Engine
    SaveService --> EconomyData
```

## 2. Padrões Arquiteturais Adotados
* **Inversão de Dependência (IoC)**: Os controladores físicos dependem de abstrações e dados injetados via `ScriptableObjects`, nunca de valores literais em código.
* **Barramento Reativo de Eventos**: Nenhuma tela de interface ou som consulta a física em polling contínuo. As informações são propagadas via eventos quando os valores sofrem mutação.
