# Relatório de Pré-Auditoria Técnica — Fase 5.9: RERY CITY (Construção da Araguari Real)

**Data:** 23/11/2026  
**Status:** Aprovado para Implementação  
**Escopo:** Transição da representação experimental para a representação cartográfica real de Araguari, MG, Brasil, mantendo o nome oficial **RERY CITY**, arquitetura modular, funcionamento 100% offline, integração aos sistemas de World Streaming e navegação viária existente.

---

## 1. Auditoria dos Sistemas Existentes e Dependências

### 1.1 World Streaming e Gestão de Chunks
- `WorldStreamer` opera em células cúbicas planas com tamanho configurável (`chunkSize = 100m`) e raio de carregamento (`loadRadiusChunks = 1` a `2`), descarregando chunks além de `unloadDistance` (220m).
- `WorldChunkCoord` mapeia posições Cartesianas `(x, z)` para inteiros `(coordX, coordZ)`.
- `WorldChunk` gerencia o estado `isLoaded` e `isVisible` dos objetos contidos.
- **Oportunidade & Regra:** A malha viária real de Araguari deve ser fatiada espacialmente em nós e trechos indexados por `WorldChunkCoord`, permitindo que apenas os segmentos viários e quarteirões próximos ao jogador estejam ativos na memória do Unity, essencial para estabilidade em dispositivos Android.

### 1.2 Navegação Viária (`TrafficNavigationManager` & `TrafficWaypointNetwork`)
- A Fase 5.1 implementou uma rede vetorial sólida: `TrafficLane`, `TrafficWaypoint`, `TrafficNode`, `TrafficSignal` e `TrafficIntersection`.
- **Diretriz Estrita:** NÃO duplicar sistemas de navegação. A malha viária real de Araguari deve gerar ou converter diretamente para instâncias compatíveis com `TrafficLane` e `TrafficWaypoint`, preservando a IA de veículos civis (Fase 5.2), perseguições policiais (Fases 5.3 e 5.5) e ocorrências (Fase 5.4).

### 1.3 Geração Procedural e Quarteirões (`ProceduralEnvironmentGenerator`)
- O gerador atual suporta quarteirões com semente determinística. Na Fase 5.9, ele será estendido para respeitar polígonos e lotes urbanos mapeados a partir dos eixos viários reais de Araguari, evitando sobreposição com o leito das ruas reais.

---

## 2. Cartografia e Fontes Geográficas Reais de Araguari (MG)

### 2.1 Marco Zero (Origem Geográfica do Mundo)
- **Local:** Praça Manoel Bonito (Centro Histórico de Araguari).
- **Latitude de Referência:** `-18.649100°` S.
- **Longitude de Referência:** `-48.190300°` O.
- **Altitude Média:** `970m`.
- **Orientação:** Eixo X = Leste geográfico (+East), Eixo Z = Norte geográfico (+North), Eixo Y = Elevação (+Up).

### 2.2 Projeção Cartográfica Planar Local
Para evitar perda de precisão em ponto flutuante de precisão simples (`float` do Unity / PhysX) sem sofrer distorções em uma área urbana de até 15 km $\times$ 15 km, utilizamos a projeção planar equidistante local ajustada pelo cosseno da latitude média ($\phi_0 = -18.6491^\circ$):

$$R_{\text{Terra}} = 6.378.137\text{ m}$$

$$x = (\lambda - \lambda_0) \cdot \frac{\pi}{180} \cdot R_{\text{Terra}} \cdot \cos(\phi_0)$$

$$z = (\phi - \phi_0) \cdot \frac{\pi}{180} \cdot R_{\text{Terra}}$$

Onde:
* $\phi$ = Latitude do ponto, $\lambda$ = Longitude do ponto.
* $\phi_0$ = Latitude da Praça Manoel Bonito (`-18.649100`).
* $\lambda_0$ = Longitude da Praça Manoel Bonito (`-48.190300`).

### 2.3 Principais Vias, Bairros e Pontos Notáveis Integrados
- **Bairros Mapeados:** Centro, Bosque, Goiás, Miranda, Amorim, Santa Helena, Sibipiruna.
- **Eixos Viários Principais:**
  1. Av. Cel. Theodolino Pereira de Araújo (Arterial de Mão Dupla / Canal Central).
  2. Av. Minas Gerais (Arterial conectora Norte-Sul).
  3. Av. Mato Grosso (Coletora Leste-Oeste).
  4. Av. Batalhão Mauá (Acesso Noroeste / Militar).
  5. Rua Rui Barbosa (Comercial / Centro Histórico).
  6. Rua Marciano Santos (Conexão Praça Manoel Bonito).
  7. Rua Brasil Accioly (Coletora Sul-Norte).
- **Pontos de Interesse (POIs):**
  - `POI_PracaManoelBonito`: Marco Zero (X: 0.0, Z: 0.0).
  - `POI_PalacioDosFerroviarios`: Sede Histórica (X: +320m, Z: +180m).
  - `POI_PracaDaMatriz`: Igreja Matriz Senhor Bom Jesus (X: -140m, Z: +220m).
  - `POI_BosqueJohnKennedy`: Reserva Florestal Urbana (X: -850m, Z: -420m).
  - `POI_MercadoMunicipal`: Centro Comercial Tradicional (X: +180m, Z: -120m).

---

## 3. Alocação Determinística de GUIDs da Fase 5.9

| Componente / Arquivo | Tipo | GUID Determinístico |
| :--- | :--- | :--- |
| `GeoCoordinateConverter.cs` | Script Utilitário / Core | `100000000000000000000000000000ac` |
| `GeoOriginConfig.cs` | ScriptableObject Definição | `100000000000000000000000000000ad` |
| `GeoPointOfInterest.cs` | MonoBehaviour Componente | `100000000000000000000000000000ae` |
| `RealRoadSegment.cs` | MonoBehaviour / Data | `100000000000000000000000000000af` |
| `RealRoadNetworkManager.cs` | MonoBehaviour Central | `100000000000000000000000000000b0` |
| `AraguariCityDataCatalog.cs` | ScriptableObject / Dados | `100000000000000000000000000000b1` |
| `GeoOrigin_AraguariCentro.asset`| ScriptableObject Asset | `200000000000000000000000000000a8` |
| `AraguariCityCatalog_Default.asset` | ScriptableObject Asset | `200000000000000000000000000000a9` |

---

## 4. Estratégia de Testes e Validação

1. **`test_araguari_world.py`**:
   - Validação da precisão da projeção planar local (erro milimétrico em distâncias de até 5 km).
   - Validação de conversão bidirecional (Lat/Lon $\leftrightarrow$ Unity X/Z).
   - Verificação de conectividade viária, larguras, sentidos (mão única/dupla) e limites de velocidade.
   - Indexação e compatibilidade com células de streaming `WorldChunkCoord`.
   - Compatibilidade de nós viários reais com `TrafficWaypoint` e `TrafficLane`.
   - Integridade estática de arquivos `.meta` e identificadores determinísticos.
2. **Regressão Consolidada:**
   - Execução das 9 suítes (Fases 5.1 a 5.9) com zero regressões.
