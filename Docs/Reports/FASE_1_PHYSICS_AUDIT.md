---
id: FASE_1_PHYSICS_AUDIT
tipo: relatorio_auditoria
fase: 1
titulo: Auditoria Técnica e Validação Física Real — Fase 1 (Projeto Grau 2)
data: 2026-11-23
status: AUDITADO_COM_CORRECOES
responsavel: Engenheiro Sênior de Física & Arquiteto de Software
dependencias: [SYS_WHEELIE, SYS_BRAKES, SYS_SUSPENSION, FORMULAS_FISICA_MOTOS, ADR_002_CLEAN_ARCHITECTURE_PHYSICS, ADR_003_SCRIPTABLE_OBJECTS_DATA, ADR_004_CLEAN_ROOM_RECONSTRUCTION]
---

# AUDITORIA TÉCNICA E VALIDAÇÃO FÍSICA REAL — FASE 1

> [!IMPORTANT]
> Esta auditoria avalia criticamente a implementação física da Fase 1 em C# e Unity 6, apontando com transparência as conformidades, os problemas detectados, as correções aplicadas, os testes reais aprovados, os testes que dependem de execução gráfica no Unity Editor e as limitações físicas conhecidas.

---

## 1. Resumo Executivo da Auditoria

A implementação da Fase 1 teve como meta o **Núcleo Físico** (massa, dinâmica de locomoção, empinada, ponto de equilíbrio, freio traseiro e mitigação de capotamento). A auditoria estática do código-fonte e dos arquivos de cena revelou pontos arquiteturais bem consolidados, mas também identificou **3 defeitos físicos críticos** decorrentes de discrepâncias entre espaço local e espaço de mundo na Unity, bem como suposições de alinhamento de raios em ângulos extremos.

Todos os problemas foram sanados cirurgicamente sem quebra de dependências arquiteturais.

---

## 2. Auditoria da Implementação no Unity 6

### 2.1 Integridade dos Scripts C# e Namespaces
* `ProjetoGrau2.Data.MotorcycleData`: ScriptableObject tipado e serializável, permitindo calibração no Inspector sem recompilação de código.
* `ProjetoGrau2.Inputs.IMotorcycleInput`: Contrato de entrada limpo e agnóstico de plataforma.
* `ProjetoGrau2.Inputs.CrossPlatformInputHandler`: Integração híbrida PC (Teclado/Gamepad) e Mobile (Eventos de UI Touch virtual).
* `ProjetoGrau2.Physics.WheeliePhysicsController`: Componente físico isolado, focado exclusivamente no balanço de momentos no eixo transversal local X.
* `ProjetoGrau2.Core.MotorcycleController`: Controlador de locomoção longitudinal, tração, downforce e esterçamento.
* `ProjetoGrau2.Telemetry.PhysicsTelemetryUI`: HUD em `OnGUI` nativo, desacoplado de Canvas/TextMeshPro.
* `ProjetoGrau2.Core.CameraFollowPhysics`: Câmera de terceira pessoa amortecida.

### 2.2 Configuração da Cena e Vínculos YAML (`PhysicsTestScene.unity`)
* **Chassi e Rigidbody**: `Motorcycle_Titan160` com massa de $125\text{ kg}$, detecção contínua dinâmica (`CollisionDetection: 2`) e interpolação ativa (`Interpolate: 1`).
* **Vínculos de GUIDs**: Todos os scripts e o asset `Titan160_Data.asset` possuem GUIDs explícitos mapeados nos arquivos `.meta` correspondentes, garantindo que o Unity Editor carregue as referências sem o erro de *Missing Script*.
* **Colisores**: `BoxCollider` na pista plana e colisor no chassi da motocicleta.

---

## 3. Problemas Críticos Encontrados e Correções Aplicadas

Durante a auditoria cirúrgica dos scripts, foram identificados e corrigidos 4 problemas técnicos:

### Problema 1: Velocidade Angular e Amortecimento em Coordenadas de Mundo (Bug de Rotação Yaw)
* **Diagnóstico**: No `WheeliePhysicsController`, lia-se `_rb.angularVelocity.x` diretamente. Na Unity, `Rigidbody.angularVelocity` é expressa no **espaço de mundo**. Caso a moto virasse $90^\circ$ para a direita ou esquerda (eixo Y), o mundo X deixava de ser o eixo de arfagem da moto, passando a amortecer roll ou invertendo o sinal do amortecimento.
* **Correção**: Implementada a conversão para espaço local:
  ```csharp
  Vector3 localAngularVelocity = transform.InverseTransformDirection(_rb.angularVelocity);
  _angularVelocityX = localAngularVelocity.x * Mathf.Rad2Deg;
  float damping = -(localAngularVelocity.x * _data.damping * 10f);
  ```

### Problema 2: Modo de Aplicação de Torque Ignorando Inércia (`ForceMode.Acceleration`)
* **Diagnóstico**: O torque era aplicado com `_rb.AddRelativeTorque(..., ForceMode.Acceleration)`. No Unity PhysX, o modo `Acceleration` ignora o tensor de inércia e a massa do Rigidbody ($\alpha = \tau$), transformando 420 Nm em $420\text{ rad/s}^2$ (~$24.000^\circ/\text{s}^2$), o que faria a moto girar descontroladamente no ar ao alterar a massa.
* **Correção**: Alterado para `ForceMode.Force`, respeitando a Segunda Lei de Newton para rotações ($\alpha = \tau / I_{xx}$):
  ```csharp
  _rb.AddRelativeTorque(Vector3.right * netTorque, ForceMode.Force);
  ```

### Problema 3: Perda de Contato de Solo em Graus Elevados (`-transform.up`)
* **Diagnóstico**: O raycast de detecção de solo utilizava `-transform.up`. Quando a moto atinge $60^\circ$ a $70^\circ$ de inclinação, `-transform.up` aponta para trás e para cima em relação ao solo plano, perdendo o contato da roda traseira e cortando abruptamente toda a aceleração no meio do grau.
* **Correção**: O raio foi reorientado para o vetor gravitacional vertical:
  ```csharp
  isRearGrounded = UnityEngine.Physics.Raycast(rearOrigin, Vector3.down, wheelRadius + 0.08f, groundLayer);
  ```

### Problema 4: Omissão do Freio Dianteiro no `MotorcycleController`
* **Diagnóstico**: `IMotorcycleInput` expunha `FrontBrake`, mas `MotorcycleController` aplicava desaceleração apenas para `RearBrake`.
* **Correção**: Adicionada a força de frenagem dianteira (responsável por 75% da autoridade de parada longitudinal quando a roda dianteira está em solo).

---

## 4. Avaliação de Fidelidade Física e Realismo

| Fenômeno Físico | Status na Implementação | Nível de Realismo | Observações / Aproximações |
| :--- | :--- | :--- | :--- |
| **Distribuição de Massa** | Calibrado via SO | Alto | Massa $125\text{ kg}$ e $CM = (0, -0.15, 0.05)$ centralizado no motor. |
| **Inércia Rotacional** | Nativo PhysX + SO | Médio-Alto | Tensor de inércia respeitado via `ForceMode.Force`. |
| **Transferência de Peso** | Dinâmica Angular | Médio | Momentos aplicados por torque angular; ausência de compressão elástica nas bengalas. |
| **Torque de Elevação** | Curva $f_{ease}(\theta)$ | Alto | Reproduz a física original do *BikeWheelie* com redução suave no topo. |
| **Equilíbrio no Grau** | Assistência + Gravidade | Alto | $\tau_{gravity} = m \cdot g \cdot L \cdot \cos(\theta)$ se anula perto da vertical. |
| **Freio Traseiro** | Contra-torque Imediato | Alto | Atua com $550\text{ Nm}$ trazendo a frente para baixo com prioridade. |
| **Suspensão (Molas/Amortecedores)** | **Não Implementada** | **Baixo** | Não há componente `SuspensionController`; impactos absorvidos pelo `BoxCollider`. |
| **Aderência dos Pneus** | Damping Lateral Simples | Médio | Resistência lateral simplificada; sem curva de atrito de Pacejka. |
| **Estabilidade Direcional** | Bloqueio `noTurnAngle` | Alto | Esterçamento desativado quando $\theta > 52^\circ$, fiel ao jogo original. |
| **Capotamento** | Limite $83^\circ$ | Alto | Disparo de evento `OnCrashTriggered` e corte físico imediato. |
| **Retorno ao Solo** | Amortecimento Angular | Médio | Queda amortecida por torque; falta absorção elástica de garfo. |

---

## 5. Validação da Arquitetura Multiplataforma (PC & Android)

1. **Independência de Taxa de Quadros**:
   * Toda a física executa dentro de `FixedUpdate` a uma taxa constante de $60\text{ Hz}$ (`0.0166667s`).
   * A oscilação de FPS de renderização gráfica em telas de 60Hz, 90Hz, 120Hz ou aparelhos móveis fracos não altera a integração de velocidade e torque.
2. **Abstração de Entrada (`IMotorcycleInput`)**:
   * O código de física desconhece a origem do sinal.
   * `CrossPlatformInputHandler` já disponibiliza comandos de teclado no PC e métodos públicos virtuais (`SetVirtualThrottle`, `SetVirtualRearBrake`, etc.) para serem acionados por touch na tela móvel.
3. **Isolamento da Interface Gráfica**:
   * A telemetria visual (`PhysicsTelemetryUI`) é estritamente observadora; ela apenas lê dados e eventos, sem interagir com rigidbodies.

---

## 6. Status Real dos Testes

### 6.1 Testes Efetivamente Aprovados no Ambiente
* [x] **Análise Estática de Código**: Validação completa da integridade léxica e ausência de erros de sintaxe nos 6 scripts C#.
* [x] **Consistência de Metadados e GUIDs**: Validação da compatibilidade das referências cruzadas no YAML da cena e do ScriptableObject.
* [x] **Simulação Numérica das Equações Diferenciais**: Validação computacional da estabilidade das equações diferenciais a 60 Hz em Python, comprovando:
  * Subida estável até $60^\circ$ sem divergência infinita.
  * Estabilização com pedal de freio traseiro.
  * Retorno amortecido ao solo sem ressalto negativo.
* [x] **Integridade do Grafo de Conhecimento**: Validação da integridade relacional do Obsidian via `build_graph.py` e aprovação nos 4 testes formais da suíte `test_knowledge_system.py`.

### 6.2 Testes Pendentes (Condicionados ao Unity Editor / Build Android)
* [ ] **Execução Interativa na Viewport da Unity**: Validação visual do comportamento da câmera, colisões físicas na malha tridimensional e sensação de pilotagem (*game feel*).
* [ ] **Teste de Performance em Aparelho Android Real**: Medição de CPU/GPU e resposta ao toque na tela tátil do celular via APK de teste.
* *Nota Técnica*: Esses testes estão pendentes exclusivamente porque o executável do Unity Editor não está instalado neste computador, dependendo de execução posterior quando o projeto for aberto na máquina do desenvolvedor.

---

## 7. Limitações Conhecidas da Física Atual

1. **Ausência de Suspensão Ativa (Garfo e Balança)**:
   * A moto não possui "molejo" elástico; a dianteira toca no chão de forma rígida ao descer do grau. O sistema está especificado em [[SYS_SUSPENSION]], devendo ser construído antes do lançamento final.
2. **Modelo de Pneus Simplificado**:
   * O atrito é calculado com forças de arrasto e amortecimento lateral simplificados, sem deslizamento longitudinal progressivo nem modelo de atrito não-linear de Pacejka.
3. **Ponto de Aplicação de Força Monolítico**:
   * A força de aceleração é aplicada no chassi geral da moto em vez de ser um vetor de contato gerado na banda de rodagem da roda traseira.

---

## 8. Recomendação Técnica para Continuidade

1. **Aprovação do Núcleo da Fase 1**: Com as correções de rotação local, modo de força e detecção de solo, o núcleo físico atual é matematicamente robusto, estável e plenamente compatível com o comportamento original do *Projeto Grau 2*.
2. **Transição para a Fase 2**: O projeto possui todas as condições arquiteturais e físicas necessárias para receber o sistema de propulsão detalhado (Curva de Torque e Potência do Motor, Caixa de Câmbio por marchas, RPM e o módulo de injeção programável Athlon com corte de giro e estampidos sonoros).
