# SYS_ARAGUARI_GEOGRAPHY — Estrutura Geográfica Real de Araguari (MG)

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.9 (Construção da Cidade Real de Araguari)

---

## 1. Visão Geral e Contexto Cartográfico

A ambientação oficial de **RERY CITY** é inspirada na cidade real de **Araguari, Minas Gerais, Brasil**, preservando sua toponímia oficial, relevo, divisões censitárias e malha urbana histórica.

### Fontes Cartográficas Oficiais e Licenças
- **OpenStreetMap contributors (ODbL):** Traçados de vias, nós de intersecção, sentidos viários e polígonos de praças.
- **IBGE (Instituto Brasileiro de Geografia e Estatística):** Coordenadas de referência do município (-18.6510° S, -48.1854° O), limites distritais e altimetria média (~970m).
- **Licenciamento:** Respeito integral aos termos da *Open Database License (ODbL)* para dados derivados do OpenStreetMap.

---

## 2. Bairros Urbanos Integrados

| Bairro | Tipologia Urbana | Centroide Latitude | Centroide Longitude | Raio Médio |
| :--- | :--- | :--- | :--- | :--- |
| **Centro** | Comercial / Histórico | `-18.649100°` | `-48.190300°` | 600m |
| **Bosque** | Residencial / Ambiental | `-18.653000°` | `-48.196000°` | 800m |
| **Goiás** | Misto / Ferroviário | `-18.642000°` | `-48.185000°` | 750m |
| **Miranda** | Residencial | `-18.646000°` | `-48.198000°` | 700m |
| **Amorim** | Residencial | `-18.658000°` | `-48.188000°` | 850m |
| **Santa Helena** | Residencial | `-18.638000°` | `-48.192000°` | 650m |
| **Sibipiruna** | Misto | `-18.644000°` | `-48.178000°` | 700m |

---

## 3. Rastreabilidade de Assets e Dados

- **Catálogo Central:** `Assets/ScriptableObjects/World/AraguariCityCatalog_Default.asset` (`200000000000000000000000000000a9`)
- **Script Definição:** `Assets/Scripts/World/Geography/AraguariCityDataCatalog.cs` (`100000000000000000000000000000b1`)
