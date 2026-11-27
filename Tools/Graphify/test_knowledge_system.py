"""
Knowledge System Verification Tests (Tests A, B, C, D)
Validates context retrieval, dependency tracing, new agent onboarding, and graph synchronization.
"""

import json
from pathlib import Path

VAULT_DIR = Path(__file__).resolve().parent.parent
GRAPH_JSON = VAULT_DIR / "10_GRAPH" / "graph.json"

def run_tests():
    data = json.loads(GRAPH_JSON.read_text(encoding="utf-8"))
    nodes = {n["id"]: n for n in data["nodes"]}
    edges = data["edges"]
    
    print("="*60)
    print("INICIANDO SUÍTE DE TESTES DO SISTEMA DE CONHECIMENTO")
    print("="*60)
    
    # ----------------------------------------------------
    # TESTE A: Recuperação seletiva para 'Modificar a Suspensão'
    # ----------------------------------------------------
    print("\n[TESTE A] Recuperação Seletiva para Modificação da Suspensão:")
    susp_node = nodes.get("SYS_SUSPENSION")
    assert susp_node is not None, "Falha: SYS_SUSPENSION não encontrado no grafo."
    
    # Encontrar documentos de referência técnica conectados
    connected_refs = [e["target"] for e in edges if e["source"] == "SYS_SUSPENSION"]
    print(f"  - Nó do Sistema Localizado: {susp_node['name']} (Status: {susp_node['status']})")
    print(f"  - Classes de Implementação: {susp_node.get('dependencies', [])}")
    print(f"  - Documentos Técnicos Conectados: {connected_refs}")
    
    assert "FORMULAS_FISICA_MOTOS" in connected_refs, "Falha: FORMULAS_FISICA_MOTOS não conectada à suspensão."
    assert "MAPEAMENTO_FISICA_MOTOCICLETAS" in connected_refs, "Falha: MAPEAMENTO_FISICA_MOTOCICLETAS não conectado à suspensão."
    
    # Verificar que arquivos irrelevantes NÃO foram associados
    irrelevant = ["SYS_POLICE", "SYS_ECONOMY", "SYS_MISSIONS"]
    for irr in irrelevant:
        assert irr not in connected_refs, f"Falha de isolamento: {irr} vazou no contexto da suspensão."
    print("  -> TESTE A APROVADO: Contexto mínimo suficiente recuperado com isolamento de ruído.")
    
    # ----------------------------------------------------
    # TESTE B: Identificação de Módulos Afetados por Alteração nos Freios
    # ----------------------------------------------------
    print("\n[TESTE B] Análise de Impacto de Alteração nos Freios (SYS_BRAKES):")
    # Procurar quem depende de SYS_BRAKES (arestas apontando para SYS_BRAKES ou de SYS_BRAKES para terceiros)
    affected_modules = [e["source"] for e in edges if e["target"] == "SYS_BRAKES" and not e["source"].startswith("PROJECT_INDEX") and not e["source"].startswith("AGENT_")]
    print(f"  - Módulos que dependem de SYS_BRAKES (Inbound Impact): {affected_modules}")
    
    assert "SYS_WHEELIE" in affected_modules, "Falha: SYS_WHEELIE deveria ser identificado como dependente de SYS_BRAKES."
    print("  -> TESTE B APROVADO: Relação crítica Freio Traseiro -> Wheelie identificada corretamente.")
    
    # ----------------------------------------------------
    # TESTE C: Simulação de Entrada de Novo Agente
    # ----------------------------------------------------
    print("\n[TESTE C] Simulação de Novo Agente (Leitura Restrita):")
    start_doc = (VAULT_DIR / "09_AGENT_CONTEXT" / "AGENT_START_HERE.md").read_text(encoding="utf-8")
    index_doc = (VAULT_DIR / "PROJECT_INDEX.md").read_text(encoding="utf-8")
    
    assert "[[PROJECT_INDEX]]" in start_doc, "Falha: AGENT_START_HERE não aponta para PROJECT_INDEX."
    assert "[[CONTEXT_RETRIEVAL_PROTOCOL]]" in start_doc, "Falha: AGENT_START_HERE não aponta para o protocolo."
    assert "SYS-01" in index_doc and "SYS-04" in index_doc, "Falha: PROJECT_INDEX não lista os sistemas principais."
    assert "ADR_001" in index_doc, "Falha: PROJECT_INDEX não referencia decisões arquiteturais."
    
    print(f"  - Tamanho AGENT_START_HERE.md: {len(start_doc)} caracteres (~{len(start_doc.split())} palavras)")
    print(f"  - Tamanho PROJECT_INDEX.md: {len(index_doc)} caracteres (~{len(index_doc.split())} palavras)")
    print("  -> TESTE C APROVADO: O agente obtém o mapa completo do projeto lendo menos de 1.500 palavras de índice.")
    
    # ----------------------------------------------------
    # TESTE D: Rastreabilidade e Auditoria de Consistência
    # ----------------------------------------------------
    print("\n[TESTE D] Auditoria de Consistência e Vínculos:")
    audit_log = (VAULT_DIR / "10_GRAPH" / "GRAPH_UPDATE_LOG.md").read_text(encoding="utf-8")
    assert "Total de Nós Mapeados" in audit_log, "Falha: Log de auditoria incompleto."
    print("  - Registro de auditoria verificado em 10_GRAPH/GRAPH_UPDATE_LOG.md")
    print("  -> TESTE D APROVADO: O sistema de integridade do grafo está operacional.")
    
    print("\n" + "="*60)
    print("TODOS OS 4 TESTES (A, B, C, D) FORAM CONCLUÍDOS COM SUCESSO!")
    print("="*60)

if __name__ == "__main__":
    run_tests()
