# SYS_PLAYER_INTERACTION — Sistema Central de Interação do Jogador

## 1. Visão Geral
O **Sistema Central de Interação** provê a camada unificada de comunicação entre o avatar do jogador (ou a motocicleta em controle ativo) e o ecossistema urbano de RERY CITY. Projetado com foco em dispositivos móveis Android, o sistema opera sob o paradigma de alocação zero (Zero GC) e desacoplamento via contratos e eventos C#.

## 2. Componentes e Contratos

### `IInteractable` (Interface)
Contrato universal implementado por qualquer entidade interativa no mundo aberto:
- `string InteractionId { get; }`: Identificador único da entidade.
- `string PromptText { get; }` / `string InteractionPrompt { get; }`: Texto contextual exibido na UI.
- `InteractionType Type { get; }`: Categoria de interação (`EntrarEstabelecimento`, `AbastecerCombustivel`, `ManutencaoMoto`, `ComprarPeca`, `ConversarNPC`, `ServicoBorracharia`).
- `float MaxInteractionDistance { get; }`: Raio esférico máximo permitido para acionamento.
- `bool CanInteract(GameObject interactor)`: Validação condicional (horário de abertura, estado, requisitos).
- `void Interact(GameObject interactor)`: Execução da lógica primária da interação.
- `void CancelInteraction(GameObject interactor)`: Interrupção defensiva ou saída da interação.

### `PlayerInteractionDetector` (MonoBehaviour)
Acoplado ao GameObject da motocicleta do jogador (`Player_Motorcycle_Proxy`):
- Varredura por esfera de proximidade (`Physics.OverlapSphereNonAlloc`) com buffer pré-alocado de 8 colisores.
- Throttling a cada 0.20s para economizar ciclos de CPU móvel.
- Seleção determinística do candidato mais próximo (`sqrMagnitude` mínimo dentro de `MaxInteractionDistance`).
- Bloqueio defensivo de comandos (`isInteracting = true` congela acelerador e esterçamento enquanto menus ou serviços estiverem abertos).
- Suporte duplo de acionamento: toque na UI touchscreen (Android) ou tecla `E` (PC/Desktop).

### `InteractionContextUI` (MonoBehaviour)
Integrado ao Canvas HUD principal (`Main_Player_HUD_Canvas`):
- Gerencia a exibição suave (fade in/out via `CanvasGroup.alpha`) do botão tátil mobile e texto de prompt.
- Inscreve-se nos eventos de detecção, perda e execução do `PlayerInteractionDetector`.
- Oculta o prompt automaticamente durante a realização do serviço ou interação interna.

## 3. Identificadores Determinísticos
- `IInteractable.cs`: GUID `100000000000000000000000000000bc`
- `PlayerInteractionDetector.cs`: GUID `100000000000000000000000000000bd`
- `InteractionContextUI.cs`: GUID `100000000000000000000000000000be`
