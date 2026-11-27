# SYS_INTERIOR_STREAMING — Streaming Leve de Interiores Urbanos

## 1. Visão Geral
Em dispositivos móveis (Android), renderizar mobiliário interno, bancadas e iluminação de múltiplos estabelecimentos ao mesmo tempo degrada severamente a taxa de quadros e estoura o orçamento de Draw Calls. O `EstablishmentInteriorManager` resolve essa restrição implementando um streaming espacial sob demanda baseado em esferas de proximidade e histerese, dispensando telas de carregamento artificiais.

## 2. Princípios de Arquitetura

### Ativação Baseada em Histerese
Para evitar que o interior fique ligando e desligando repetidamente ("flicker/thrashing") quando o jogador transita pela fronteira da calçada:
- **Raio de Ativação (`activationRadius`)**: Padrão de 18 a 20 metros. Ativa o interior quando o observador se aproxima.
- **Raio de Desativação (`deactivationRadius`)**: Padrão de 26 a 28 metros. O interior só é desligado após o jogador cruzar esse limiar estendido.

### Zero Garbage Collection no Runtime
- Avaliação temporal em passos regulares (`checkInterval = 0.25s`) em vez de varredura a cada frame.
- Uso de buffers e estruturas pré-alocadas (`InteriorEntry`), evitando `Instantiate`, `Destroy` ou criação de listas temporárias em `Update()`.

### Integração com o WorldStreamer
- Atua como uma subcamada local para o sistema de streaming de chunks da Fase 5.7.
- Quando o chunk é descarregado pelo `WorldStreamer`, os interiores são suprimidos automaticamente.

## 3. Identificador Determinístico
- `EstablishmentInteriorManager.cs`: GUID `100000000000000000000000000000c1`
