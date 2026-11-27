# SYS_CITY_MATERIALS — Biblioteca de Materiais Compartilhados

**Data:** 23/11/2026  
**Status:** Implementado e Verificado  
**Fase de Origem:** Fase 5.10 (Construção Visual e Arquitetônica de Araguari)

---

## 1. Visão Geral

A biblioteca de materiais compartilhados (`CityMaterialLibrary`) centraliza os shaders e materiais universais da cidade no Universal Render Pipeline (URP). Ao padronizar e compartilhar instâncias de materiais com GPU Instancing ativado, reduz-se expressivamente o número de *SetPass Calls* e *Draw Calls*, essencial para manter taxas de quadros estáveis em celulares Android.

---

## 2. Tipos de Materiais e Propriedades

| Tipo | Aplicação | Smoothness | GPU Instancing |
| :--- | :--- | :--- | :--- |
| `AsfaltoVia` | Pistas de rolamento e avenidas | `0.15` | Sim |
| `ConcretoMeioFio` | Guias, sarjetas e canteiros centrais | `0.20` | Sim |
| `CalcadaPedestre` | Calçadas de concreto e mosaico | `0.10` | Sim |
| `FachadaResidencial` | Alvenaria, tinta acrílica e reboco | `0.10` | Sim |
| `FachadaComercial` | Vidraças e revestimentos comerciais | `0.30` | Sim |
| `TelhadoColonial` | Telhas cerâmicas tipo capa e canal | `0.15` | Sim |
| `VidroJanela` | Janelas e portas translúcidas | `0.90` | Sim |
| `MetalEstrutural` | Postes de iluminação, grades e portões | `0.50` | Sim |
| `VegetacaoArvores` | Folhagens de ipês e gramados de praças | `0.05` | Sim |
| `SinalizacaoBranca` | Linhas de retenção e faixas de pedestres | `0.25` | Sim |
| `SinalizacaoAmarela` | Divisórias de mão dupla e canteiros | `0.25` | Sim |

---

## 3. Rastreabilidade de Assets e GUIDs

- **Script Biblioteca:** `Assets/Scripts/World/Architecture/CityMaterialLibrary.cs` (`100000000000000000000000000000b4`)
- **Asset Padrão:** `Assets/ScriptableObjects/World/CityMaterialLibrary_Default.asset` (`200000000000000000000000000000aa`)
