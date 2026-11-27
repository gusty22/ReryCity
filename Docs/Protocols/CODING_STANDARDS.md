---
id: CODING_STANDARDS
tipo: padrao_engenharia
projeto: Projeto Grau 2
versao: 1.0.0
ultima_atualizacao: 2026-11-23
---

# PADRÕES DE CODIFICAÇÃO E ENGENHARIA: C# & UNITY 6

> [!NOTE]
> Diretrizes para escrita de código limpo, performático para dispositivos móveis Android e manutenível por múltiplos desenvolvedores e agentes de IA.

---

## 1. Princípios de Arquitetura

1. **Separação Rígida entre Física, Visual e Interface**:
   * O código de física veicular (`WheeliePhysicsController`, `MotorcycleEngine`) roda em `FixedUpdate` e atua **exclusivamente** via forças, torques e colisores físicos (`Rigidbody.AddRelativeTorque`, `AddForce`).
   * A física **nunca** mexe diretamente em texto de UI, ponteiro de velocímetro ou ossos de animação.
2. **Desacoplamento Orientado a Eventos**:
   * Para comunicar estados (ex: rotação do motor, velocidade, ângulo de grau), os controladores físicos emitem eventos em C# nativo (`System.Action<float>`, `Action<int>`).
   * Os componentes de apresentação (HUD, velocímetro, áudio e animador IK) assinam esses eventos e reagem passivamente.
3. **Parametrização por `ScriptableObjects`**:
   * Nenhuma constante de física (ex: torque máximo, ângulo de equilíbrio, relação de marchas) deve ser escrita como valor numérico solto (*hardcoded*) dentro dos scripts.
   * Crie assets de `ScriptableObject` (`MotorcycleData`, `EngineTuningConfig`) contendo as variáveis expostas e inspecionáveis no editor.
4. **Alocação Zero no Ciclo de Atualização (*Zero GC in Update/FixedUpdate*)**:
   * Proibido instanciar objetos (`new`), concatenar strings ou chamar `GetComponent` dentro de `Update`, `FixedUpdate` ou `LateUpdate`.
   * Faça cache prévio de todas as referências no `Awake` ou `Start`.

---

## 2. Nomenclatura e Convenções em C#

* **Classes e Structs**: `PascalCase` (Ex: `MotorcycleController`, `WheeliePhysicsController`).
* **Interfaces**: Prefixadas com `I` em `PascalCase` (Ex: `IEngineTuning`, `IWheelieSensor`).
* **Métodos e Propriedades**: `PascalCase` (Ex: `CalculateLiftTorque()`, `CurrentRpm { get; private set; }`).
* **Campos Privados / Serializados**: `camelCase` com underscore inicial (Ex: `_balanceAngle`, `_rigidbody`, `_engineData`).
* **Constantes**: `UPPER_SNAKE_CASE` (Ex: `MAX_ALLOWED_PITCH_ANGLE`, `GRAVITY_MAGNITUDE`).
