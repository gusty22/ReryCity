---
id: ADR_002
tipo: adr
status: APROVADO
data: 2026-11-23
sistemas_afetados: [SYS-01, SYS-02, SYS-03, SYS-04, SYS-05, SYS-07, SYS-08, SYS-20]
---

# ADR 002: Separação Estrita de Física (Rigidbody/Torques) e Apresentação (UI/Áudio/IK)

## Contexto e Problema
No jogo original, observou-se forte acoplamento entre certas rotinas de física e elementos de cena, dificultando testes unitários automatizados e gerando bugs visuais quando a taxa de quadros (FPS) oscila.

## Decisão Adotada
Implementar uma arquitetura desacoplada onde:
1. Todos os cálculos de física (empinar, tração, frenagem, amortecimento) residem em componentes executados exclusivamente em `FixedUpdate`.
2. As forças são transmitidas ao motor de física do Unity unicamente por meio de `Rigidbody.AddRelativeTorque` e `Rigidbody.AddForce`.
3. Os subsistemas visuais (IK de braços e pernas do piloto, velocímetro, efeitos de partículas e síntese de áudio) não executam cálculos de física; apenas assinam eventos gerados pelo modelo físico.

## Consequências
* **Positivas**: Simulação determinística e estável independente da taxa de quadros; possibilidade de rodar testes de física em modo headless sem carregar gráficos; código limpo e modular.
* **Negativas**: Requer a manutenção de um barramento de eventos bem tipado.
