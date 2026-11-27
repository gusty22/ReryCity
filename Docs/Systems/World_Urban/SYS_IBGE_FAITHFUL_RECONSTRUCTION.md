# SYS_IBGE_FAITHFUL_RECONSTRUCTION — Reconstrução Geográfica Fiel de Araguari (MG)

## 1. Visão Geral
Define a arquitetura e o pipeline para reconstrução tridimensional e viária de Araguari, Minas Gerais, na Unity 6 a partir de dados cartográficos oficiais do **IBGE Censo Demográfico 2022** (`3103504_faces_de_logradouros_2022.json`).

O sistema substitui a geração de quarteirões sintéticos arbitrários por quadras poligonais reais, alinhamento exato de vias, larguras de calçadas e nomes oficiais de logradouros, preservando integralmente o pipeline visual (`BuildingGenerator`, `FacadeDetailGenerator`, `StreetInfrastructureDetailer`), o sistema de streaming (`WorldStreamer`) e a simulação de tráfego e interação.

## 2. Componentes e Estrutura de Dados

### `RealCityDataPayload` (Estruturas de Dados Desserializáveis)
- `CityGeoMetadata`: Metadados municipais, CRS de ancoragem (Praça Manoel Bonito: `-18.649100° S, -48.190300° O`, 970m alt) e limites da região piloto.
- `RealQuadraData`: Representa cada quarteirão fechado identificado no censo, com `quadra_id`, centro local em metros, AABB (`min_x`, `max_x`, `min_z`, `max_z`), dimensões métricas (`width_meters`, `length_meters`), total de domicílios e logradouros confrontantes.
- `RealFaceData`: Cada face de quadra com vértices locais `(x, z)` e globais WGS84 `(lon, lat)`, tipo e nome do logradouro (`NM_TIP_LOG`, `NM_LOG`).

### `RealRoadCorridor` (MonoBehaviour)
Define o leito carroçável e as calçadas de cada via real:
- `centerlineNodes`: Eixo central vetorial da via.
- `roadWidthMeters`: Largura do asfalto (ex: 14m em avenidas arteriais, 9m a 10m em ruas locais).
- `sidewalkWidthMeters`: Largura das calçadas (2.5m).
- `GetClosestPointOnCenterline()`: Projeção ortogonal eficiente para encaixe de veículos e viaturas policiais.

### `AraguariRealCityBuilder` (MonoBehaviour)
Gerenciador e orquestrador em runtime:
- Carrega `araguari_pilot_geometry.json` a partir de `Application.streamingAssetsPath/Geography/` de forma 100% offline.
- Mapeia quadras e faces indexadas espacialmente.
- Fornece métodos de consulta espacial (`GetQuadrasInArea()`, `GetFacesByStreet()`).

## 3. Identificadores Determinísticos
- `RealCityDataPayload.cs`: GUID `100000000000000000000000000000c3`
- `AraguariRealCityBuilder.cs`: GUID `100000000000000000000000000000c4`
- `RealRoadCorridor.cs`: GUID `100000000000000000000000000000c5`
- Dataset exportado: `Assets/StreamingAssets/Geography/araguari_pilot_geometry.json`
- Script de processamento offline: `Tools/GeoPipeline/process_araguari_ibge.py`
