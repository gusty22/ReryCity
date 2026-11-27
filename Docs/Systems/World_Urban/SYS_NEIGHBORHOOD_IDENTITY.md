# SYS_NEIGHBORHOOD_IDENTITY — Identidade Visual e Temática dos 7 Bairros

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.11 (Ambientação Urbana e Detalhamento de Araguari)

---

## 1. Visão Geral

O sistema de identidade temática diferencia visualmente as 7 regiões urbanas reais de **Araguari, MG** em **RERY CITY**. Através de parâmetros de densidade edilícia, tipos de calçamento, arborização e vocação econômica (comercial vs residencial), cada bairro adquire personalidade reconhecível.

---

## 2. Matriz de Parâmetros dos Bairros

| Bairro | Densidade Edilícia | Densidade de Árvores | Calçamento Dominante | Proporção Comercial | Média de Andares |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Centro** | `0.95` (Máxima) | 2 por 100m | Pedra Portuguesa | `0.85` (Foco Comercial) | 3.8 andares |
| **Bosque** | `0.55` (Baixa) | 12 por 100m (Reserva) | Concreto | `0.10` (Residencial) | 1.3 andares |
| **Goiás** | `0.80` (Média-Alta) | 3 por 100m | Concreto | `0.50` (Misto/Oficinas) | 2.0 andares |
| **Miranda** | `0.75` (Média) | 5 por 100m | Concreto | `0.20` (Residencial) | 1.4 andares |
| **Amorim** | `0.70` (Média) | 4 por 100m | Concreto | `0.25` (Residencial) | 1.5 andares |
| **Santa Helena** | `0.65` (Média) | 6 por 100m | Concreto | `0.15` (Residencial) | 1.2 andares |
| **Sibipiruna** | `0.70` (Média) | 5 por 100m | Concreto | `0.30` (Misto) | 1.6 andares |

---

## 3. Rastreabilidade de Assets e GUIDs

- **Script Catálogo:** `Assets/Scripts/World/Architecture/NeighborhoodThemeCatalog.cs` (`100000000000000000000000000000b8`)
- **Asset Padrão:** `Assets/ScriptableObjects/World/NeighborhoodThemes_Default.asset` (`200000000000000000000000000000ac`)
