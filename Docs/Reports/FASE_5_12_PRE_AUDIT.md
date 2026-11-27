# Relatório de Pré-Auditoria Técnica — Fase 5.12: RERY CITY (Vida Urbana, Estabelecimentos Interativos e Interação do Jogador)

**Data:** 23/11/2026  
**Status:** Aprovado para Implementação  
**Escopo:** Implementação da arquitetura centralizada de interação do jogador, estabelecimentos urbanos interativos em Araguari (Oficinas de Grau, Postos de Combustível, Borracharias e Mercadinhos), gerenciamento de interiores modulares e controles táteis contextuais para Android.

---

## 1. Diagnóstico do Estado Atual e Requisitos

### 1.1 Detecção de Proximidade e Prevenção de Conflitos
- O jogador controla a motocicleta através do `MotorcycleController` ou pilota a pé.
- **Risco & Diretriz:** A detecção de elementos interativos não pode alocar memória a cada frame (`Zero GC`). Deve utilizar `Physics.OverlapSphereNonAlloc` ou triggers com eventos `OnTriggerEnter`/`OnTriggerExit` e distância máxima configurável (2.5m a 5.0m).
- **Bloqueio de Comandos:** Durante a interação (ex: abastecendo no posto ou em menu da oficina), os comandos de acelerador, freio e direção da moto devem ser desativados defensivamente, evitando movimentações fantasmas, sendo restaurados de forma transparente ao sair da área interativa.

### 1.2 Tipos de Estabelecimentos Interativos
1. **Oficinas Mecânicas de Grau (ex: "Oficina do Grau"):**
   - Vão de entrada amplo para motos.
   - Área de parada / elevador de manutenção.
   - Ponto de interação no balcão de atendimento e bancada de ferramentas.
2. **Postos de Combustível (ex: "Posto Pioneiro"):**
   - Ilhas de abastecimento com bombas de combustível.
   - Ponto de interação para compra de combustível, debitando moedas locais no `SaveSystem` e completando o tanque da motocicleta.
3. **Comércio Local e Borracharias:**
   - Balcões e áreas acessíveis para serviços rápidos.

### 1.3 Interiores Modulares e Streaming Local
- Nenhum interior comercial deve ser carregado ou mantido ativo desnecessariamente.
- Interiores são organizados em células modulares ativadas por proximidade do jogador ou gatilho de entrada, integradas ao ciclo de descarregamento do `WorldStreamer`.

---

## 2. Alocação Determinística de GUIDs da Fase 5.12

| Componente / Arquivo | Tipo | GUID Determinístico |
| :--- | :--- | :--- |
| `IInteractable.cs` | Interface / Core | `100000000000000000000000000000bc` |
| `PlayerInteractionDetector.cs` | Script / Detecção | `100000000000000000000000000000bd` |
| `InteractionContextUI.cs` | Script / UI Contextual | `100000000000000000000000000000be` |
| `InteractiveEstablishment.cs` | Script / Estabelecimento | `100000000000000000000000000000bf` |
| `GasStationPump.cs` | Script / Bombas Posto | `100000000000000000000000000000c0` |
| `EstablishmentInteriorManager.cs`| Script / Streaming Interiores | `100000000000000000000000000000c1` |
| `InteractiveEstablishmentConfig.cs`| Script / Config ScriptableObject | `100000000000000000000000000000c2` |
| `InteractiveEstablishment_Default.asset`| ScriptableObject Asset | `200000000000000000000000000000ae` |

---

## 3. Plano de Implementação

1. Criar `IInteractable.cs` definindo o contrato de objetos interativos.
2. Implementar `PlayerInteractionDetector.cs` e `InteractionContextUI.cs` com suporte a teclado (E), gamepad e botão tátil Android.
3. Implementar `InteractiveEstablishment.cs` e `GasStationPump.cs` com lógica de serviços (abastecimento, manutenção).
4. Implementar `EstablishmentInteriorManager.cs` para ativação/desativação leve de interiores por proximidade.
5. Criar asset padrão `InteractiveEstablishment_Default.asset` e integrá-lo com o nó `Araguari_Interactive_Establishments` em `UrbanTrafficTestScene.unity`.
6. Criar `test_urban_interactions.py` e executar regressão consolidada das 12 suítes.
7. Atualizar Obsidian Vault, Graphify e emitir relatório oficial de conclusão.
