# ReryCity — Projeto Grau 2

Projeto de jogo de motocicletas em Unity 6, com foco em mundo aberto inspirado na geografia real de Araguari–MG e desempenho no Samsung Galaxy A54 5G.

> **Estado do repositório:** inicializado para receber o projeto. O código-fonte e os assets ainda precisam ser enviados a partir do diretório local `ProjetoGrau2_Unity`. Nenhuma build Android está disponível neste repositório neste momento.

## Organização esperada

O diretório raiz deste repositório deverá corresponder à **raiz do projeto Unity**:
- `Assets/`: scripts, cenas, recursos, geografia, arquivos `.meta`.
- `Packages/`: `manifest.json` e `packages-lock.json`.
- `ProjectSettings/`: configurações do projeto.
- `Tools/`: pipeline geográfico e scripts auxiliares, quando aplicável.
- `test_*.py` e `run_all_tests.py`: testes auxiliares, quando aplicável.

Não versionar `Library/`, `Temp/`, `Logs/`, `Obj/`, `Build/`, `Builds/` e `UserSettings/`: são arquivos gerados, temporários ou específicos da máquina.

## Segurança e licenças

Antes do primeiro push, revisar:
- arquivos grandes, usando Git LFS quando apropriado;
- modelos, músicas, imagens e outros assets quanto ao direito de redistribuição;
- segredos, chaves, certificados de assinatura e arquivos de ambiente;
- dados geográficos e as atribuições/licenças de suas fontes;
- caminhos e dados privados em relatórios ou documentação.

O projeto não depende da inclusão do cache `Library/` no repositório. Para compilar em nuvem, será preciso configurar um serviço com Unity Editor e licença compatíveis.

## Estado da validação

Os relatórios de testes estáticos/Python não substituem a compilação C#, a execução no Unity Editor ou a homologação da build no Galaxy A54. Essas validações permanecem etapas próprias.
