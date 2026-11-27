---
id: ADR_001
tipo: adr
status: APROVADO
data: 2026-11-23
sistemas_afetados: [TODOS]
---

# ADR 001: Adoção de Unity 6 (6000.3.6f1) e Universal Render Pipeline (URP)

## Contexto e Problema
Precisamos definir a engine e o pipeline de renderização para a reconstrução independente do Projeto Grau 2, garantindo paridade de performance com o jogo original e compatibilidade direta com os shaders e assets empacotados.

## Alternativas Consideradas
1. **Godot 4**: Excelente engine aberta e leve, porém exigiria conversão complexa e manual de todos os prefabs, malhas e animações serializadas em formato proprietário da Unity.
2. **Unreal Engine 5**: Alta fidelidade gráfica, mas peso proibitivo para dispositivos móveis Android de entrada e intermediários típicos do mercado brasileiro.
3. **Unity 6 (6000.3.6f1) com URP**: Mesma versão de engine identificada na análise do jogo original (`boot.config`), permitindo reutilização direta dos assets e garantia de 60 FPS com baixo consumo de bateria via Vulkan.

## Decisão Adotada
Adotar **Unity 6 (`6000.3.6f1`)** com **Universal Render Pipeline (URP)** como plataforma oficial de desenvolvimento.

## Consequências
* **Positivas**: Máxima performance gráfica em Android via Vulkan PSO Cache; suporte nativo a SRP Batcher; total compatibilidade com os formatos de malha e materiais extraídos.
* **Negativas**: Dependência do ecossistema Unity e exigência de compilação IL2CPP para builds de produção.
