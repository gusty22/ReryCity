# SYS_POINT_OF_INTEREST — Catálogo de Pontos de Interesse (POIs)

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.9 (Construção da Cidade Real de Araguari)

---

## 1. Visão Geral

Os Pontos de Interesse (POIs) georreferenciados identificam marcos históricos, culturais, praças, garagens mecânicas e pontos icônicos da cidade de Araguari, servindo de base para missões, áreas de encontros de motos, abastecimento e oficinas.

---

## 2. Pontos Notáveis Cadastrados

| ID do POI | Nome Oficial | Bairro | Categoria | Coordenadas WGS84 | Raio de Influência |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `POI_PracaManoelBonito` | Praça Manoel Bonito (Marco Zero) | Centro | Praça Pública / Histórico | `-18.649100, -48.190300` | 45m |
| `POI_PalacioDosFerroviarios` | Palácio dos Ferroviários (Prefeitura/EFGO) | Goiás | Marco Histórico | `-18.647500, -48.187300` | 50m |
| `POI_PracaDaMatriz` | Praça da Matriz (Senhor Bom Jesus) | Centro | Praça Pública | `-18.647800, -48.191600` | 40m |
| `POI_BosqueJohnKennedy` | Bosque John Kennedy (Reserva Urbana) | Bosque | Parque / Reserva | `-18.653500, -48.198000` | 120m |
| `POI_MercadoMunicipal` | Mercado Municipal de Araguari | Centro | Comércio Central | `-18.650200, -48.188600` | 35m |

---

## 3. Rastreabilidade de Assets e GUIDs

- **Script POI:** `Assets/Scripts/World/Geography/GeoPointOfInterest.cs` (`100000000000000000000000000000ae`)
- **GameObject na Cena:** `POI_PracaManoelBonito` (`fileID: 1193` em `UrbanTrafficTestScene.unity`).
