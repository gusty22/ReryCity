"""
Relational Knowledge Graph Builder for Projeto Grau 2 (Graphify Engine)
Parses YAML frontmatter, wikilinks [[...]], and explicit dependencies in the Obsidian Vault.
Exports:
- RELATIONSHIP_INDEX.md (Markdown index of all forward and backward dependencies)
- graph.json (GraphRAG-ready node/edge topology)
- graph.html (Interactive Force-Directed Visual Graph)
- GRAPH_UPDATE_LOG.md (Consistency audit trail: broken links, orphan nodes)
"""

import os
import re
import json
from pathlib import Path
import networkx as nx

VAULT_DIR = Path(__file__).resolve().parent.parent

def parse_markdown_file(file_path):
    rel_path = file_path.relative_to(VAULT_DIR).as_posix()
    content = file_path.read_text(encoding="utf-8")
    
    # Extract YAML frontmatter
    metadata = {}
    if content.startswith("---"):
        parts = content.split("---", 2)
        if len(parts) >= 3:
            yaml_text = parts[1]
            for line in yaml_text.strip().split("\n"):
                if ":" in line:
                    key, val = line.split(":", 1)
                    key = key.strip()
                    val = val.strip()
                    if val.startswith("[") and val.endswith("]"):
                        items = [x.strip() for x in val[1:-1].split(",") if x.strip()]
                        metadata[key] = items
                    else:
                        metadata[key] = val
    
    node_id = metadata.get("id", file_path.stem)
    node_type = metadata.get("tipo", "documento")
    node_system = metadata.get("sistema", "")
    node_status = metadata.get("status", "")
    dependencies = metadata.get("dependencias", [])
    if isinstance(dependencies, str):
        dependencies = [dependencies]
        
    # Extract wikilinks [[Target]] or [[Target|Alias]]
    wikilinks = []
    matches = re.findall(r'\[\[(.*?)\]\]', content)
    for m in matches:
        target = m.split("|")[0].split("#")[0].strip()
        if target and target != node_id:
            wikilinks.append(target)
            
    return {
        "id": node_id,
        "name": file_path.stem,
        "path": rel_path,
        "type": node_type,
        "system": node_system,
        "status": node_status,
        "dependencies": dependencies,
        "wikilinks": list(set(wikilinks)),
        "content_length": len(content)
    }

def main():
    G = nx.DiGraph()
    files_data = {}
    
    # 1. Scan all markdown files
    for md_file in VAULT_DIR.rglob("*.md"):
        if ".obsidian" in md_file.parts:
            continue
        data = parse_markdown_file(md_file)
        files_data[data["id"]] = data
        files_data[data["name"]] = data  # alias by stem
        G.add_node(data["id"], **data)
        
    # 2. Add edges (explicit dependencies + wikilinks)
    broken_links = []
    for node_id, data in files_data.items():
        if node_id != data["id"]:
            continue
        # From explicit dependencies
        for dep in data["dependencies"]:
            if dep in files_data:
                target_id = files_data[dep]["id"]
                G.add_edge(node_id, target_id, relation="depends_on")
            else:
                G.add_node(dep, id=dep, name=dep, type="external_or_unresolved", status="MISSING")
                G.add_edge(node_id, dep, relation="depends_on")
                broken_links.append((node_id, dep, "unresolved_dependency"))
                
        # From wikilinks
        for link in data["wikilinks"]:
            if link in files_data:
                target_id = files_data[link]["id"]
                G.add_edge(node_id, target_id, relation="references")
            else:
                G.add_node(link, id=link, name=link, type="unresolved_link", status="MISSING")
                G.add_edge(node_id, link, relation="references")
                broken_links.append((node_id, link, "unresolved_wikilink"))
                
    # 3. Generate RELATIONSHIP_INDEX.md
    index_md_path = VAULT_DIR / "10_GRAPH" / "RELATIONSHIP_INDEX.md"
    lines = [
        "---",
        "id: RELATIONSHIP_INDEX",
        "tipo: indice_relacional",
        "projeto: Projeto Grau 2",
        "---",
        "",
        "# ÍNDICE RELACIONAL DE CONHECIMENTO (GRAPHIFY MAP)",
        "> **Mapa Topológico de Conexões entre Sistemas, Classes, Decisões e Testes.**",
        "",
        f"**Métricas do Grafo**: {G.number_of_nodes()} nós | {G.number_of_edges()} arestas de dependência/referência.",
        "",
        "## 1. Conexões dos Subsistemas de Jogo (Forward & Backward Dependencies)",
        ""
    ]
    
    systems = sorted([n for n, d in G.nodes(data=True) if d.get("system") or n.startswith("SYS_")])
    for sys in systems:
        data = G.nodes[sys]
        deps_out = list(G.successors(sys))
        deps_in = list(G.predecessors(sys))
        
        lines.append(f"### `[[{sys}]]` — {data.get('name', sys)}")
        lines.append(f"* **Tipo / Status**: `{data.get('type', 'N/A')}` | `{data.get('status', 'N/A')}`")
        if deps_out:
            out_str = ", ".join([f"`[[{d}]]`" for d in deps_out])
            lines.append(f"* **Depende de (Outbound)**: {out_str}")
        else:
            lines.append("* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)")
            
        if deps_in:
            in_str = ", ".join([f"`[[{d}]]`" for d in deps_in])
            lines.append(f"* **Impacta Diretamente (Inbound)**: {in_str}")
        else:
            lines.append("* **Impacta Diretamente (Inbound)**: Nenhum consumidor registrado")
        lines.append("")
        
    lines.append("## 2. Mapa Rápido de Impacto por Subsistema")
    lines.append("| Se você alterar... | Você DEVE verificar os seguintes módulos afetados: |")
    lines.append("| :--- | :--- |")
    for sys in ["SYS_BRAKES", "SYS_SUSPENSION", "SYS_WHEELIE", "SYS_ENGINE"]:
        if sys in G:
            affected = [f"[[{p}]]" for p in G.predecessors(sys) if not p.startswith("PROJECT_INDEX") and not p.startswith("AGENT_")]
            lines.append(f"| `[[{sys}]]` | {', '.join(affected) if affected else 'Nenhum módulo direto'} |")
            
    index_md_path.write_text("\n".join(lines), encoding="utf-8")
    
    # 4. Export graph.json
    graph_json_path = VAULT_DIR / "10_GRAPH" / "graph.json"
    nodes_export = [{"id": n, **d} for n, d in G.nodes(data=True)]
    edges_export = [{"source": u, "target": v, **d} for u, v, d in G.edges(data=True)]
    graph_json_path.write_text(json.dumps({"nodes": nodes_export, "edges": edges_export}, indent=2, ensure_ascii=False), encoding="utf-8")
    
    # 5. Export interactive graph.html
    graph_html_path = VAULT_DIR / "10_GRAPH" / "graph.html"
    html_content = f"""<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="utf-8">
<title>Grafo de Conhecimento: Projeto Grau 2</title>
<script src="https://d3js.org/d3.v7.min.js"></script>
<style>
  body {{ margin: 0; background: #121214; color: #e1e1e6; font-family: -apple-system, sans-serif; overflow: hidden; }}
  #header {{ position: absolute; top: 15px; left: 20px; z-index: 10; pointer-events: none; }}
  h1 {{ margin: 0; font-size: 18px; color: #ff9000; }}
  p {{ margin: 5px 0; font-size: 12px; color: #a8a8b3; }}
  .node text {{ font-size: 10px; fill: #e1e1e6; pointer-events: none; }}
  .link {{ stroke: #29292e; stroke-opacity: 0.8; stroke-width: 1.5px; }}
  .link.depends_on {{ stroke: #ff9000; stroke-width: 2px; }}
</style>
</head>
<body>
<div id="header">
  <h1>Projeto Grau 2 — Grafo Relacional de Conhecimento</h1>
  <p>{G.number_of_nodes()} nós mapeados | {G.number_of_edges()} conexões topológicas</p>
</div>
<svg width="100%" height="100vh"></svg>
<script>
const data = {json.dumps({"nodes": nodes_export, "edges": edges_export})};
const width = window.innerWidth, height = window.innerHeight;
const svg = d3.select("svg").attr("viewBox", [0, 0, width, height]);
const g = svg.append("g");
svg.call(d3.zoom().scaleExtent([0.1, 4]).on("zoom", (e) => g.attr("transform", e.transform)));

const simulation = d3.forceSimulation(data.nodes)
  .force("link", d3.forceLink(data.edges).id(d => d.id).distance(80))
  .force("charge", d3.forceManyBody().strength(-200))
  .force("center", d3.forceCenter(width / 2, height / 2));

const link = g.append("g").selectAll("line").data(data.edges).enter().append("line")
  .attr("class", d => "link " + (d.relation || ""));

const node = g.append("g").selectAll(".node").data(data.nodes).enter().append("g").attr("class", "node")
  .call(d3.drag().on("start", dragstarted).on("drag", dragged).on("end", dragended));

node.append("circle").attr("r", d => d.id.startsWith("SYS_") ? 9 : 5)
  .attr("fill", d => d.id.startsWith("SYS_") ? "#ff9000" : (d.id.startsWith("ADR_") ? "#04d361" : "#8257e6"));

node.append("text").attr("dx", 10).attr("dy", ".35em").text(d => d.name || d.id);

simulation.on("tick", () => {{
  link.attr("x1", d => d.source.x).attr("y1", d => d.source.y).attr("x2", d => d.target.x).attr("y2", d => d.target.y);
  node.attr("transform", d => `translate(${{d.x}},${{d.y}})`);
}});

function dragstarted(event) {{ if (!event.active) simulation.alphaTarget(0.3).restart(); event.subject.fx = event.subject.x; event.subject.fy = event.subject.y; }}
function dragged(event) {{ event.subject.fx = event.x; event.subject.fy = event.y; }}
function dragended(event) {{ if (!event.active) simulation.alphaTarget(0); event.subject.fx = null; event.subject.fy = null; }}
</script>
</body>
</html>"""
    graph_html_path.write_text(html_content, encoding="utf-8")
    
    # 6. Generate GRAPH_UPDATE_LOG.md
    log_path = VAULT_DIR / "10_GRAPH" / "GRAPH_UPDATE_LOG.md"
    log_content = [
        "# REGISTRO DE ATUALIZAÇÃO DO GRAFO (GRAPH AUDIT LOG)",
        "",
        "## Auditoria de Consistência e Vínculos",
        f"- **Total de Nós Mapeados**: {G.number_of_nodes()}",
        f"- **Total de Conexões Direcionadas**: {G.number_of_edges()}",
        f"- **Densidade do Grafo**: {nx.density(G):.4f}",
        f"- **Nós Órfãos (sem conexões)**: {len(list(nx.isolates(G)))}",
        "",
        "## Status dos Artefatos Gerados",
        "- `RELATIONSHIP_INDEX.md`: Atualizado com sucesso.",
        "- `graph.json`: Exportado em formato de adjacência compatível com GraphRAG.",
        "- `graph.html`: Exportado com renderizador interativo D3.js.",
        ""
    ]
    log_path.write_text("\n".join(log_content), encoding="utf-8")
    
    print(f"Graph build completed: {G.number_of_nodes()} nodes, {G.number_of_edges()} edges.")

if __name__ == "__main__":
    main()
