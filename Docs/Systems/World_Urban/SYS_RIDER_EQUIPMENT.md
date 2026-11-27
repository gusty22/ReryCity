---
id: SYS_RIDER_EQUIPMENT
tipo: sistema
codigo: SYS-11-EQUIP
status: IMPLEMENTADO_EM_FASE_4
responsavel: Artista Técnico & Engenheiro de Customização
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_RIDER, SYS_WORKSHOP, RiderEquipmentData]
arquivos_relacionados: [RiderEquipmentManager.cs, RiderEquipmentData.cs]
referencia_tecnica: [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE EQUIPAMENTOS E VESTIMENTAS DO PILOTO (SYS-11-EQUIP)

## 1. Identificação e Objetivo
* **Código**: `SYS-11-EQUIP`
* **Nome**: Customização de Equipamentos e Roupas (*Rider Equipment Manager*).
* **Status**: `IMPLEMENTADO_EM_FASE_4`.
* **Objetivo**: Permitir equipar, substituir e remover capacetes, camisas de time, bermudas, tênis esportivos, chinelos e mochilas de entrega sem necessidade de reconstruir o esqueleto ou o `AnimatorController`.

## 2. Categorias Suportadas
1. `Helmet`: Capacetes San Marino, modelos fechados e balaclava.
2. `Shirt`: Camisetas de futebol, regatas brancas e blusas corta-vento.
3. `Pants`: Bermudas jeans rasgadas e bermudas de tactel.
4. `Footwear`: Tênis esportivos e chinelos de dedo.
5. `Gloves`: Luvas de manobras.
6. `Accessories`: Mochila térmica de entregas (*bag*) e óculos espelhados.
