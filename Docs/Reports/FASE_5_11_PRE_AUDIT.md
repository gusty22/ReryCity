# Relatório de Pré-Auditoria Técnica — Fase 5.11: RERY CITY (Ambientação Urbana e Detalhamento de Araguari)

**Data:** 23/11/2026  
**Status:** Aprovado para Implementação  
**Escopo:** Ambientação detalhada e contextualizada dos 7 bairros urbanos reais de Araguari, MG, com fachadas brasileiras, estabelecimentos comerciais fictícios, muros, garagens, mobiliário viário de calçadas (meio-fio, lombadas, numeração predial) e otimização prioritária para Android.

---

## 1. Diagnóstico do Estado Atual e Requisitos

### 1.1 Coerência Geográfica e Loteamento (Fases 5.9 e 5.10)
- Os eixos viários reais (`Av. Cel. Theodolino`, `Av. Minas Gerais`, etc.) e o Marco Zero na Praça Manoel Bonito fornecem a base cartográfica.
- O sistema de lotes (`UrbanLot`) garante que o recuo frontal impeça sobreposições com a pista de rolamento e calçadas.
- **Evolução da Fase 5.11:** Adicionar detalhes volumétricos e ornamentos arquitetônicos brasileiros (portões basculantes e de correr, muretas, calçadas com meio-fio rebaixado para garagens, marquises de lojas e letreiros comerciais fictícios) sem violar a restrição de não-invasão da via.

### 1.2 Identidade Diferenciada dos 7 Bairros
Cada bairro possui vocação e características visuais próprias:
1. **Centro:** Alta densidade, comércio intenso, edifícios mistos (2 a 5 andares), iluminação abundante, calçadas movimentadas e letreiros comerciais.
2. **Bosque:** Baixa densidade, perfil residencial arborizado, casas de médio/alto padrão com recuo ajardinado e proximidade do Bosque John Kennedy.
3. **Goiás:** Perfil histórico e misto, galpões, armazéns, proximidade da ferrovia e oficinas mecânicas.
4. **Miranda:** Bairro residencial consolidado, casas térreas com muro e garagens frontais.
5. **Amorim:** Bairro residencial de expansão com ruas mais largas e comércio local.
6. **Santa Helena:** Casas residenciais familiares, padarias e pequenos mercados.
7. **Sibipiruna:** Bairro de trânsito moderado, pequenos comércios e residências populares.

### 1.3 Marcas Fictícias Exclusivas de RERY CITY
Para respeitar as diretrizes legais e consolidação de identidade própria, todos os estabelecimentos comerciais recebem marcas fictícias originais:
- *"Oficina do Grau"* (Preparação mecânica de motos)
- *"Borracharia do Baiano"* (Reparo rápido de pneus e câmaras)
- *"Mercadinho Central"* (Secos e molhados)
- *"Auto Peças Mauá"* (Peças e reposição)
- *"Posto Pioneiro"* (Combustíveis e conveniência)
- *"Padaria Pão de Queijo"* (Lanches e café regional mineiro)

---

## 2. Alocação Determinística de GUIDs da Fase 5.11

| Componente / Arquivo | Tipo | GUID Determinístico |
| :--- | :--- | :--- |
| `NeighborhoodThemeCatalog.cs` | Script / Definição de Bairros | `100000000000000000000000000000b8` |
| `FacadeDetailGenerator.cs` | Script / Gerador de Detalhes | `100000000000000000000000000000b9` |
| `StreetInfrastructureDetailer.cs`| Script / Calçadas e Vias | `100000000000000000000000000000ba` |
| `CommercialSignboardCatalog.cs` | Script / Catálogo de Letreiros | `100000000000000000000000000000bb` |
| `NeighborhoodThemes_Default.asset`| ScriptableObject Asset | `200000000000000000000000000000ac` |
| `CommercialSignboards_Default.asset`| ScriptableObject Asset | `200000000000000000000000000000ad` |

---

## 3. Plano de Implementação

1. Implementar `NeighborhoodThemeCatalog.cs` e criar `NeighborhoodThemes_Default.asset` com a calibração temática dos 7 bairros.
2. Implementar `CommercialSignboardCatalog.cs` e criar `CommercialSignboards_Default.asset` com o acervo de marcas e letreiros fictícios.
3. Implementar `FacadeDetailGenerator.cs` para aplicação de elementos decorativos (muros, portões, marquises, letreiros) com determinismo por semente.
4. Implementar `StreetInfrastructureDetailer.cs` para posicionamento de meio-fios, lombadas, faixas de retenção e numeração de imóveis.
5. Integrar o nó `Araguari_City_Detail_Root` na cena `UrbanTrafficTestScene.unity`.
6. Desenvolver a suíte `test_city_environment_detail.py` e executar a regressão completa das 11 suítes via `run_all_tests.py`.
7. Atualizar Obsidian Vault, Graphify Knowledge Graph e emitir documentação de conclusão.
