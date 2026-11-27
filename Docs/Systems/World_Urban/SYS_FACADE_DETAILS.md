# SYS_FACADE_DETAILS — Detalhamento de Fachadas e Estabelecimentos Brasileiros

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.11 (Ambientação Urbana e Detalhamento de Araguari)

---

## 1. Visão Geral

O `FacadeDetailGenerator` transforma blocos volumétricos em fachadas urbanas com características da arquitetura brasileira: muros de alvenaria com portões de garagem (vão mínimo de 3.5m) e letreiros comerciais fictícios originais de **RERY CITY**.

---

## 2. Marcas Comerciais Fictícias Integradas

Para garantir total conformidade legal e identidade autônoma, foram estabelecidas marcas originais:

| ID | Nome Fantasia | Slogan / Atividade | Categoria |
| :--- | :--- | :--- | :--- |
| `SIGN_OficinaDoGrau` | Oficina do Grau | Preparação, Escapamentos & Acertos Athlon | Oficina Mecânica |
| `SIGN_BorrachariaBaiano`| Borracharia do Baiano | Pneus, Câmaras & Socorro 24 Horas | Oficina / Borracharia |
| `SIGN_MercadinhoCentral`| Mercadinho Central | Secos e Molhados - Tradição em Araguari | Comércio |
| `SIGN_AutoPecasMaua` | Auto Peças Mauá | Linha Completa para Motos e Veículos | Comércio / Autopeças |
| `SIGN_PadariaPaoDeQueijo`| Padaria Pão de Queijo de Minas | Café Colonial & Quitandas Artesanais | Padaria / Alimentação |
| `SIGN_PostoPioneiro` | Posto Pioneiro | Combustível com Qualidade e Ducha Grátis | Posto de Combustível |

---

## 3. Rastreabilidade de Assets e GUIDs

- **Script Letreiros:** `Assets/Scripts/World/Architecture/CommercialSignboardCatalog.cs` (`100000000000000000000000000000bb`)
- **Script Gerador:** `Assets/Scripts/World/Architecture/FacadeDetailGenerator.cs` (`100000000000000000000000000000b9`)
- **Asset Padrão:** `Assets/ScriptableObjects/World/CommercialSignboards_Default.asset` (`200000000000000000000000000000ad`)
