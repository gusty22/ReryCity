# SYS_INTERACTIVE_ESTABLISHMENTS — Estabelecimentos Comerciais e Bombas de Combustível

## 1. Visão Geral
Define as regras, configurações e componentes para os pontos comerciais de RERY CITY, tais como oficinas mecânicas, borracharias, mercadinhos e postos de combustíveis. Todos os custos e serviços são integrados à economia local persistida via `SaveSystem`.

## 2. Componentes

### `InteractiveEstablishmentConfig` (ScriptableObject)
Define os metadados do ponto comercial:
- `establishmentId`: Identificador único.
- `establishmentName`: Nome comercial exibido ao jogador.
- `category`: Categoria de atividade (`Workshop`, `GasStation`, `TireShop`, `ConvenienceStore`, `AutoParts`, `Landmark`).
- `neighborhoodName`: Bairro de Araguari onde o estabelecimento está instalado.
- `worldPosition` e `interactionRadius`: Ponto de ancoragem e alcance.
- `fuelPricePerLiterCentavos`, `repairCostPerPercentCentavos`, `tireMaintenanceCostCentavos`.
- `availableServices`: Lista de serviços ofertados.
- `isOpen24Hours`, `openHour`, `closeHour`: Horário de atendimento com suporte a plantão noturno.

### `InteractiveEstablishment` (MonoBehaviour, `IInteractable`)
Gerencia o ciclo de interação com o imóvel:
- Pontos de referência para vaga de moto (`parkingSpot`), elevador/bancada (`serviceBay`) e balcão (`customerCounter`).
- Transição de entrada/saída (`isPlayerInside`) com ativação/desativação de interiores.
- Estacionamento assistido: `ParkVehicle()` anula velocidade inercial (`linearVelocity = 0`, `angularVelocity = 0`) e alinha a motocicleta na vaga.
- Débito no `SaveSystem`: `RequestService()` valida fundos em `playerMoneyCentavos`, deduz o valor e salva as alterações em disco.

### `GasStationPump` (MonoBehaviour, `IInteractable`)
Bomba de abastecimento de combustível ("Posto Pioneiro"):
- Litragem dinâmica com preço unitário configurável (`pricePerLiterCentavos`).
- `RefuelLiters()`, `RefuelFullTank()`, `RefuelByValueCentavos()`.
- Abastecimento adaptativo: caso o jogador não tenha saldo para o tanque total, calcula e injeta a quantidade exata suportada pela carteira disponível.
- Emite eventos `OnRefuelStarted`, `OnRefuelCompleted(liters, costCentavos)` e `OnRefuelStopped`.

## 3. Identificadores Determinísticos
- `InteractiveEstablishmentConfig.cs`: GUID `100000000000000000000000000000c2`
- `InteractiveEstablishment.cs`: GUID `100000000000000000000000000000bf`
- `GasStationPump.cs`: GUID `100000000000000000000000000000c0`
- `InteractiveEstablishment_Default.asset`: GUID `200000000000000000000000000000ae`
