---
id: DOCUMENTATION_RULES
tipo: padrao_documentacao
projeto: Projeto Grau 2
versao: 1.0.0
ultima_atualizacao: 2026-11-23
---

# REGRAS E MODELOS DE DOCUMENTAÇÃO TÉCNICA

> [!NOTE]
> Regras para manter o Vault do Obsidian consistente, navegável e interoperável entre humanos e agentes de IA.

---

## 1. Regras Fundamentais de Redação

1. **Frontmatter YAML Obrigatório**: Todo arquivo `.md` criado no Vault deve começar com o bloco de propriedades YAML:
   ```yaml
   ---
   id: IDENTIFICADOR_UNICO
   tipo: sistema | subsistema | classe | adr | teste | tarefa
   sistema: CODIGO_DO_SISTEMA (ex: SYS-01)
   status: PLANEJADO | ESPECIFICADO | EM_DESENVOLVIMENTO | IMPLEMENTADO | TESTADO | CONCLUIDO | BLOQUEADO | DEPRECADO
   responsavel: NOME_OU_AGENTE
   data_criacao: AAAA-MM-DD
   ultima_atualizacao: AAAA-MM-DD
   dependencias: [SYS-XX, SYS-YY]
   arquivos_relacionados: [caminho/do/script.cs]
   ---
   ```
2. **Utilização de Links Bidirecionais do Obsidian**:
   * Sempre que referenciar outro documento, use a sintaxe de wikilink: `[[NOME_DO_ARQUIVO]]` ou `[[NOME_DO_ARQUIVO|Texto de Exibição]]`.
3. **Não Duplicação de Conhecimento (Princípio DRY)**:
   * Cada fato técnico deve residir em **apenas um** documento principal.
   * Os demais documentos que mencionarem aquele tópico devem apontar para a fonte primária via wikilink, nunca copiar e colar trechos inteiros.
4. **Distinção Rígida entre Fato e Hipótese**:
   * O que foi comprovado por evidência (metadados, código extraído, teste físico) deve ser explicitado como **Comprovado**.
   * Suposições ou extrapolações devem receber alerta do tipo `> [!WARNING] Hipótese Pendente de Validação`.

---

## 2. Modelos de Documentação (Templates)

### Modelo de Documento de Sistema (`SYS_XX.md`)
```markdown
---
id: SYS_XX
tipo: sistema
status: ESPECIFICADO
dependencias: [SYS_YY]
---
# SISTEMA: [NOME DO SISTEMA]

## 1. Identificação e Objetivo
* **Código**: SYS-XX
* **Finalidade**: [Descrição do que o sistema faz]

## 2. Requisitos e Regras de Negócio
* REQ-01: [Regra]
* REQ-02: [Regra]

## 3. Arquitetura e Classes Responsáveis
* `ClassePrincipal.cs`: [Função]
* `IContrato.cs`: [Interface]

## 4. Diagrama de Fluxo e Dependências
[Mermaid Diagram]

## 5. Eventos e Interfaces
* `OnEventRaised`: [Quando dispara e dados transmitidos]

## 6. Testes e Critérios de Aceitação
* Ver [[PLANO_DE_TESTES_FISICOS]]
```
