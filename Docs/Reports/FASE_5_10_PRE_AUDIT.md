# Relatório de Pré-Auditoria Técnica — Fase 5.10: RERY CITY (Construção Visual e Arquitetônica de Araguari)

**Data:** 23/11/2026  
**Status:** Aprovado para Implementação  
**Escopo:** Implementação da arquitetura visual tridimensional e loteamento urbano inspirado em Araguari, MG, com respeito estrito à malha viária real (Fase 5.9), streaming espacial, perfis móveis de iluminação e otimização de materiais para Android.

---

## 1. Diagnóstico do Estado Atual e Dependências

### 1.1 Malha Viária Real e Zonas de Não-Sobreposição
- A Fase 5.9 mapeou eixos viários reais (`Av. Cel. Theodolino`, `Av. Minas Gerais`, `Av. Mato Grosso`, etc.) com larguras reais entre 10m e 16m e calçadas entre 2.0m e 2.5m.
- **Risco & Diretriz:** A geração de edifícios, muros e lotes não pode avançar sobre a caixa de rolamento das ruas nem sobre os cruzamentos. Cada lote urbano (`UrbanLot`) possui um recuo frontal obrigatório (*setback*) calculado a partir da meia-largura da via e da calçada:
  $$\text{Recuo Frontal Mínimo} = \frac{\text{Largura da Via}}{2} + \text{Largura da Calçada} + \text{Margem de Segurança (0.5m)}$$

### 1.2 Integração com World Streaming e Chunks
- Os edifícios e itens de mobiliário urbano devem ser instanciados como nós filhos ou agrupados na hierarquia da célula de streaming correspondente (`WorldChunkCoord`).
- Chunks descarregados pelo `WorldStreamer` devem liberar ou desativar automaticamente as geometrias e colisores associados, mantendo o consumo de memória RAM dentro dos limites seguros de dispositivos móveis Android (< 1.5 GB alocados).

### 1.3 Iluminação e Perfis de Desempenho Mobile
- A iluminação pública (postes e luminárias urbanas) deve ser sensível ao `MobileEnergyProfileManager`:
  - `Economico`: Luzes dinâmicas de postes desativadas (ou apenas emissive texture em shaders), sem sombras em tempo real.
  - `Equilibrado`: Luzes dinâmicas pontuais agrupadas com raio reduzido e sombras duras limitadas aos objetos imediatos.
  - `AltoDesempenho`: Luzes pontuais com atenuação completa e sombras suaves.

---

## 2. Alocação Determinística de GUIDs da Fase 5.10

| Componente / Arquivo | Tipo | GUID Determinístico |
| :--- | :--- | :--- |
| `UrbanLot.cs` | Script / Estrutura de Lote | `100000000000000000000000000000b2` |
| `BuildingGenerator.cs` | Script / Gerador Modular | `100000000000000000000000000000b3` |
| `CityMaterialLibrary.cs` | ScriptableObject / Materiais | `100000000000000000000000000000b4` |
| `UrbanFurnitureManager.cs` | Script / Mobiliário e Luzes | `100000000000000000000000000000b5` |
| `UrbanLightingController.cs` | Script / Ciclo e Perfis | `100000000000000000000000000000b6` |
| `CityMaterialLibrary_Default.asset`| ScriptableObject Asset | `200000000000000000000000000000aa` |
| `BuildingArchetypeCatalog_Default.asset` | ScriptableObject Asset | `200000000000000000000000000000ab` |

---

## 3. Plano de Implementação

1. **Lotes e Arquitetura Modular (`UrbanLot`, `BuildingGenerator`)**:
   - Criação da estrutura de dados de lote urbano com dimensões, orientação, estilo arquitetônico e cálculo de perímetro.
   - Gerador procedural de edifícios respeitando os lotes e quarteirões com semente determinística.
2. **Biblioteca de Materiais Compartilhados (`CityMaterialLibrary`)**:
   - Centralização de referências de materiais para garantir *Static Batching* e *GPU Instancing* no URP (asfalto, concreto, fachadas, telhados cerâmicos coloniais/modernos, vidro, calçadas portuguesas/concreto e metais).
3. **Mobiliário Urbano e Iluminação (`UrbanFurnitureManager`, `UrbanLightingController`)**:
   - Posicionamento de postes, placas de sinalização viária, faixas de pedestres, árvores urbanas (ipês/oitis típicos do Triângulo Mineiro) e pontos de ônibus.
   - Controle dinâmico do estado das luzes públicas por horário e perfil de energia.
4. **Integração na Cena (`UrbanTrafficTestScene.unity`)**:
   - Instanciação do nó `Araguari_Visual_Architecture` conectado aos gerenciadores da cidade.
5. **Testes Automatizados (`test_city_visual_infrastructure.py`) e Regressão**:
   - Criação da suíte cobrindo verificação de colisão/invasão de leito viário, determinismo, alternância de iluminação e integridade de GUIDs.
   - Execução de todas as 10 suítes (Fases 5.1 a 5.10).
