---
id: GRAPHIFY_GUIDE
tipo: guia_ferramenta
projeto: Projeto Grau 2
---

# GUIA DO GRAPHIFY E MAPA RELACIONAL DE CONHECIMENTO

> [!NOTE]
> Este guia explica como a ferramenta Graphify e o motor de grafos topológicos do Projeto Grau 2 são utilizados para mapear e recuperar relacionamentos entre sistemas, classes, dependências e decisões.

---

## 1. O Que É o Grafo de Conhecimento do Projeto
O Grafo de Conhecimento é uma representação direcionada $G = (V, E)$ onde:
* **Vértices ($V$)**: Representam entidades do projeto:
  * Sistemas de jogo (`SYS_WHEELIE`, `SYS_SUSPENSION`, `SYS_BRAKES`, etc.).
  * Documentos técnicos e especificações de engenharia reversa.
  * Decisões arquiteturais (`ADR_001`, `ADR_002`, etc.).
  * Classes C# e ScriptableObjects.
* **Arestas ($E$)**: Representam conexões tipadas entre entidades:
  * `depends_on`: Dependência técnica direta (ex: `SYS_WHEELIE` depende de `SYS_BRAKES`).
  * `references`: Menção ou citação contextual em wikilinks (`[[...]]`).

---

## 2. Como Utilizar o Grafo para Recuperação Seletiva

### Pergunta 1: *"Se eu alterar a suspensão, o que pode quebrar?"*
* **Ação**: Consulte a seção *Mapa Rápido de Impacto por Subsistema* no [[RELATIONSHIP_INDEX]].
* **Resposta do Grafo**: O nó `SYS_SUSPENSION` possui arestas de entrada vindas de `SYS_WHEELIE` (Física do Grau). Logo, ao alterar o curso ou dureza da suspensão, o ângulo de subida e a estabilização do grau sofrerão impacto direto e devem ser testados.

### Pergunta 2: *"Quais documentos tratam das fórmulas do motor?"*
* **Ação**: Inspecione as conexões de saída de `SYS_ENGINE`.
* **Resposta do Grafo**: Conecta diretamente a [[FORMULAS_FISICA_MOTOS]] e [[MAPEAMENTO_FISICA_MOTOCICLETAS]].

---

## 3. Como Reconstruir e Auditar o Grafo

Sempre que novos documentos ou sistemas forem adicionados ao Vault, execute o script de sincronização com o interpretador Python 3.13:

```powershell
& "$env:LOCALAPPDATA\Programs\Python\Python313\python.exe" "C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Knowledge\10_GRAPH\build_graph.py"
```

O script irá:
1. Re-escanear todo o cofre em busca de novos nós e wikilinks.
2. Identificar links órfãos ou quebrados e registrar em [[GRAPH_UPDATE_LOG]].
3. Atualizar [[RELATIONSHIP_INDEX]], `graph.json` e o visualizador interativo D3.js `graph.html`.

---

## 4. Visualização Interativa do Grafo
Abra o arquivo `10_GRAPH/graph.html` em qualquer navegador web para interagir com o grafo de forças em tempo real, arrastando nós e inspecionando clusters de dependências.
