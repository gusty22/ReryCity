---
id: ADR_004
tipo: adr
status: APROVADO
data: 2026-11-23
sistemas_afetados: [TODOS]
---

# ADR 004: Engenharia Reversa Clean-Room e Proibição de Cópia Direta de Código

## Contexto e Problema
A análise do binário `libil2cpp.so` e do `global-metadata.dat` revelou toda a arquitetura e nomes de variáveis do jogo original. É necessário estabelecer uma barreira legal e de engenharia para que o novo projeto seja 100% legítimo, autônomo e seguro para distribuição comercial.

## Decisão Adotada
Adotar a metodologia rigorosa de **Engenharia Limpa (*Clean-Room Development*)**:
1. A investigação dos metadados originais serve estritamente como especificação funcional e levantamento de requisitos físicos/mecânicos.
2. Todo o código C# da nova implementação deve ser escrito do zero, utilizando padrões modernos de software (SOLID, DDD, ScriptableObjects).
3. Não haverá cópia ou transposição de código descompilado do binário original para o repositório da nova implementação.

## Consequências
* **Positivas**: Total segurança jurídica e de propriedade intelectual; código mais moderno, limpo e testável do que o código legado da versão original.
* **Negativas**: Requer a formulação independente e validação dos algoritmos físicos via testes práticos.
