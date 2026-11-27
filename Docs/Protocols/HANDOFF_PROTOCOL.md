---
id: HANDOFF_PROTOCOL
tipo: protocolo_obrigatorio
projeto: Projeto Grau 2
versao: 1.0.0
ultima_atualizacao: 2026-11-23
---

# PROTOCOLO DE TRANSFERÊNCIA DE SESSÃO (HANDOFF PROTOCOL)

> [!IMPORTANT]
> Ao concluir qualquer tarefa ou ao final de cada sessão de trabalho, o Agente de IA é **obrigado** a emitir um relatório estruturado de encerramento seguindo o modelo abaixo.  
> Esse registro garante que um próximo agente, em uma nova sessão ou janela de contexto, saiba exatamente onde o projeto parou sem precisar reanalisar tudo.

---

## Modelo Padronizado de Handoff

```markdown
### 📋 RELATÓRIO DE HANDOFF (TRANSFERÊNCIA DE SESSÃO)

1. **Objetivo Recebido**:
   * Descrição clara e concisa da tarefa que o usuário solicitou.

2. **Contexto Consultado**:
   * Quais códigos de sistema (ex: SYS-01, SYS-04) e documentos foram lidos.

3. **Arquivos Examinados**:
   * Caminhos relativos dos arquivos inspecionados para a tarefa.

4. **Arquivos Criados**:
   * Lista de novos scripts, documentos ou assets gerados.

5. **Arquivos Modificados**:
   * Lista de arquivos alterados e resumo das alterações realizadas em cada um.

6. **Implementações Realizadas**:
   * Funcionalidades que foram efetivamente codificadas ou especificadas.

7. **Testes Executados & Resultados**:
   * Quais casos de teste foram executados (unitários, integração física, etc.).
   * Resultados obtidos (Sucesso / Falha / Em validação).

8. **Documentação e Relacionamentos Atualizados**:
   * Quais documentos do Vault e nós do Grafo tiveram status ou vínculos ajustados.

9. **Pendências Conhecidas**:
   * O que não pôde ser concluído ou ficou bloqueado por dependência externa.

10. **Próxima Ação Recomendada**:
    * Passo exato e cirúrgico a ser executado pelo próximo agente.
```

---

## Arquivo de Histórico Cumulativo

Sempre que um handoff for gerado, além de ser apresentado ao usuário no fechamento do turno, o agente deve registrar um resumo em:
👉 `08_CHANGELOG/CHANGELOG.md`
