---
id: PROJECT_RULES
tipo: regras_projeto
projeto: Projeto Grau 2
---

# REGRAS FUNDAMENTAIS DO PROJETO

1. **Clean-Room e Legalidade**:
   * É estritamente proibido descompilar código C++ original e colá-lo na nova base de código.
   * Toda nova implementação deve ser escrita do zero, orientada por contratos, especificações funcionais e equações matemáticas limpas.
2. **Preservação de Evidências**:
   * Nunca modifique, renomeie ou sobrescreva os arquivos originais extraídos em `REA/extracted_apks/` ou no dump `com.ProjetoGrau2`.
3. **Sem Código Sem Documentação**:
   * Nenhuma classe de física ou funcionalidade de jogo deve ser criada sem que o respectivo documento de sistema (`SYS_XX.md`) esteja atualizado.
4. **Respeito aos Critérios de Conclusão**:
   * Uma funcionalidade só é considerada `CONCLUIDA` quando atender aos 6 critérios de aceitação: especificação, código funcional, testes físicos executados, rastreabilidade, documentação e ausência de efeitos colaterais.
5. **Autonomia com Registro Obrigatório**:
   * Agentes de IA podem propor refatorações e melhorias, desde que registrem a decisão em um ADR (`06_DECISIONS/ADR/`) e atualizem o [[PROJECT_INDEX]].
