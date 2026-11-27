---
id: PROJECT_INDEX
tipo: indice_mestre
projeto: Projeto Grau 2
versao: 1.0.0
ultima_atualizacao: 2026-11-23
responsavel: Arquiteto de Software / Agentes IA
---

# PROJECT INDEX — PROJETO GRAU 2
> **Ponto de Entrada Obrigatório para Todos os Agentes de IA e Desenvolvedores.**  
> Antes de executar qualquer tarefa, consulte [[AGENT_START_HERE]] e siga o [[CONTEXT_RETRIEVAL_PROTOCOL]].

---

## 1. Identificação Geral do Projeto

* **Nome Oficial do Produto**: **RERY CITY**
* **Identificador Interno de Desenvolvimento**: Projeto Grau 2 (`com.InsanityGroup.ProjetoGrau2`)
* **Objetivo Geral**: Desenvolver um jogo e simulador urbano independente de motocicletas focado na cultura brasileira do "grau" (acrobacias e manobras em uma roda só), ambientado em uma representação futurista inspirada em Araguari, MG, com física realista de suspensão e tração, preparação mecânica (injeção programável Athlon, dinamômetro e escape esportivo), mundo aberto procedural com streaming, inteligência artificial de trânsito civil, perseguição policial coordenada e funcionamento 100% offline.
* **Engine & Runtime**: Unity 6 (`6000.3.6f1`), C# (.NET Standard 2.1 / C# 9+), Universal Render Pipeline (URP).
* **Plataformas Alvo**: Android (Mobile prioritário) e Windows PC (Desktop).
* **Localização do Vault Obsidian**: `C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Knowledge`
* **Repositório de Evidências de Engenharia Reversa**: `C:\Users\washi\OneDrive\Área de Trabalho\REA\extracted_apks`

---

## 2. Mapa Estrutural do Conhecimento (Vault)

```
ProjetoGrau2_Knowledge/
├── PROJECT_INDEX.md                  <- Você está aqui (Índice Central)
├── README.md                         <- Boas-vindas e introdução ao Vault
├── 00_PROJECT/                       <- Visão executiva, roadmap e regras do projeto
├── 01_ARCHITECTURE/                  <- Arquitetura de software, dependências e fluxos
├── 02_SYSTEMS/                       <- Documentação técnica individual por subsistema
├── 03_TECHNICAL_REFERENCE/           <- Fórmulas físicas, pesquisa IL2CPP e catálogo de assets
├── 04_IMPLEMENTATION/                <- Código implementado, cenas e prefabs do Unity
├── 05_TESTING/                       <- Planos de teste físico, resultados e bug tracker
├── 06_DECISIONS/ADR/                 <- Registros de Decisões Arquiteturais (ADRs)
├── 07_TASKS/                         <- Gestão de tarefas (Backlog, In Progress, Completed)
├── 08_CHANGELOG/                     <- Histórico de versões e alterações
├── 09_AGENT_CONTEXT/                 <- Protocolos para Agentes de IA e padrões de código
├── 10_GRAPH/                         <- Mapa relacional de conhecimento e Graphify
└── 99_ARCHIVE/                       <- Arquivos legados e versões congeladas
```

---

## 3. Matriz de Sistemas e Status Atual de Desenvolvimento

Os status oficiais são: `PLANEJADO`, `ESPECIFICADO`, `EM_DESENVOLVIMENTO`, `IMPLEMENTADO`, `TESTADO`, `CONCLUIDO`, `BLOQUEADO`, `DEPRECADO`.

| Código | Subsistema | Documentação | Status | Dependências Diretas |
| :--- | :--- | :--- | :--- | :--- |
| **SYS-01** | **Física do Grau (Wheelie)** | [[SYS_WHEELIE]], [[FASE_1_PHYSICS_AUDIT]] | `AUDITADO_COM_CORRECOES` | SYS-02 (Engine), SYS-05 (Brakes), Rigidbody |
| **SYS-02** | **Motor e Dinamômetro** | [[SYS_ENGINE]] | `IMPLEMENTADO_EM_FASE_2` | SYS-03 (Transmission), ScriptableObjects |
| **SYS-03** | **Câmbio e Embreagem** | [[SYS_TRANSMISSION]] | `IMPLEMENTADO_EM_FASE_2` | SYS-02 (Engine) |
| **SYS-04** | **Suspensão (Molejo)** | [[SYS_SUSPENSION]] | `IMPLEMENTADO_EM_FASE_3` | Rigidbody, WheelCollider / Raycast |
| **SYS-05** | **Freio Duplo (Mão/Pé)** | [[SYS_BRAKES]] | `ESPECIFICADO` | SYS-01 (Wheelie), Rigidbody |
| **SYS-06** | **Injeção Programável (Athlon)**| [[SYS_ATHLON]], [[SYS_ATHLON_MOBILE]] | `IMPLEMENTADO_EM_FASE_3` | SYS-02 (Engine), SYS-08 (Audio) |
| **SYS-07** | **Orquestrador de Moto** | [[SYS_MOTORCYCLE]] | `ESPECIFICADO` | SYS-01 a SYS-06 |
| **SYS-08** | **Áudio Granular & Roncos** | [[SYS_AUDIO]], [[SYS_AUDIO_INTEGRATION]] | `IMPLEMENTADO_EM_FASE_5_6` | SYS-02 (Engine), SYS-06 (Athlon), Sirenes e Colisão |
| **SYS-09** | **Câmeras Dinâmicas (Cinemachine)**| [[SYS_CAMERA]] | `IMPLEMENTADO_EM_FASE_3` | SYS-07 (Motorcycle) |
| **SYS-10** | **Entradas (Input System)** | [[SYS_INPUT]], [[SYS_MOBILE_CONTROLS]] | `IMPLEMENTADO_EM_FASE_5_6` | Unity Input System, Touchscreen Mobile Android |
| **SYS-11** | **Piloto e IK (Humanoid)** | [[SYS_RIDER]], [[SYS_RIDER_IK]], [[SYS_RIDER_ANIMATIONS]], [[SYS_RIDER_RAGDOLL]], [[SYS_RIDER_EQUIPMENT]] | `IMPLEMENTADO_EM_FASE_4` | SYS-07 (Motorcycle), Unity IK |
| **SYS-12** | **Polícia e Perseguição (IA)**| [[SYS_POLICE_AI]], [[SYS_POLICE_PURSUIT]], [[SYS_TRAFFIC_VIOLATIONS]], [[SYS_WANTED_LEVEL]], [[SYS_POLICE_HUD]], [[SYS_POLICE_DISPATCH]], [[SYS_PURSUIT_COORDINATION]] | `IMPLEMENTADO_EM_FASE_5_5` | IA Tática, ROCAM, Infrações, Wanted Level, Interceptação, Despacho COPOM, Coordenação Multi-Viatura |
| **SYS-13** | **Tráfego Urbano & Veículos Civis** | [[SYS_TRAFFIC_NAVIGATION]], [[SYS_AI_VEHICLES]], [[SYS_AI_DRIVER]], [[SYS_AI_SENSORS]], [[SYS_TRAFFIC_SPAWNING]], [[SYS_TRAFFIC_OPTIMIZATION]], [[SYS_ROAD_BLOCKAGE]] | `IMPLEMENTADO_EM_ETAPA_5_2` | Rede Vetorial, Veículos NPC, Física Rigidbody, Object Pooling, Bloqueios e Desvios |
| **SYS-14** | **Missões e Entregas (Delivery)**| [[SYS_MISSIONS]] | `PLANEJADO` | SYS-17 (Economy), SYS-18 (Celular/GPS) |
| **SYS-15** | **Oficina e Garagem** | [[SYS_WORKSHOP]] | `IMPLEMENTADO_EM_FASE_3` | SYS-16 (Customization), SYS-17 (Economy) |
| **SYS-16** | **Customização Estética/Som** | [[SYS_CUSTOMIZATION]] | `IMPLEMENTADO_EM_FASE_3` | SYS-07 (Motorcycle), SYS-08 (Audio) |
| **SYS-17** | **Economia e Saldo (Centavos)** | [[SYS_ECONOMY]] | `ESPECIFICADO` | Persistência Segura |
| **SYS-18** | **Celular Virtual e GPS (A\*)** | [[SYS_PHONE_GPS]], [[SYS_ATHLON_MOBILE]] | `ESPECIFICADO` | SYS-14 (Missions), SYS-17 (Economy) |
| **SYS-19** | **Mundo Aberto, Streaming & População** | [[SYS_WORLD]], [[SYS_WORLD_STREAMING]], [[SYS_PROCEDURAL_ENVIRONMENT]], [[SYS_WORLD_POPULATION]], [[SYS_WORLD_OPTIMIZATION]], [[SYS_WORLD_EVENTS]], [[SYS_ROAD_BLOCKAGE]], [[SYS_POLICE_DISPATCH]] | `IMPLEMENTADO_EM_FASE_5_7` | Chunks, Streaming, Geração Determinística, Pedestres, LOD, Time-Slicing |
| **SYS-20** | **Interface, HUD e Menus** | [[SYS_UI]], [[SYS_MOTORCYCLE_HUD]], [[SYS_MOBILE_CONTROLS]], [[SYS_POLICE_HUD]] | `IMPLEMENTADO_EM_FASE_5_6` | HUD Moto, Tacômetro, Eventos, Touchscreen, UI Policial |
| **SYS-21** | **Persistência e Save Game** | [[SYS_SAVE_SYSTEM]] | `IMPLEMENTADO_EM_FASE_3` | SYS-17 (Economy), ScriptableObjects |
| **SYS-22** | **Identidade RERY CITY, Boot Offline & Energia** | [[SYS_GAME_IDENTITY]], [[SYS_OFFLINE_INITIALIZATION]], [[SYS_SIMULATED_REWARDS]], [[SYS_GAME_LOADING]], [[SYS_ENERGY_MANAGEMENT]] | `IMPLEMENTADO_EM_FASE_5_8` | Configuração Central, Boot Desacoplado, Recompensa Local, Perfis Mobile 30/60 FPS |
| **SYS-23** | **Geografia Real de Araguari & Malha Viária** | [[SYS_ARAGUARI_GEOGRAPHY]], [[SYS_GEOGRAPHIC_COORDINATES]], [[SYS_REAL_ROAD_NETWORK]], [[SYS_MAP_STREAMING_INTEGRATION]], [[SYS_POINT_OF_INTEREST]] | `IMPLEMENTADO_EM_FASE_5_9` | Projeção WGS84, Marco Zero Manoel Bonito, Vias Reais, Bairros e Streaming de Chunks |
| **SYS-24** | **Construção Visual & Arquitetura de Araguari** | [[SYS_URBAN_LOTS_BUILDINGS]], [[SYS_CITY_MATERIALS]], [[SYS_URBAN_FURNITURE_LIGHTING]] | `IMPLEMENTADO_EM_FASE_5_10` | Lotes Urbanos, Gerador de Edifícios, Arquétipos, Materiais URP e Iluminação Pública |
| **SYS-25** | **Ambientação Urbana & Detalhamento de Bairros** | [[SYS_NEIGHBORHOOD_IDENTITY]], [[SYS_FACADE_DETAILS]], [[SYS_STREET_INFRASTRUCTURE]] | `IMPLEMENTADO_EM_FASE_5_11` | Temática de 7 Bairros, Marcas Fictícias, Muros, Garagens, Lombadas e Meio-Fio |
| **SYS-26** | **Interação Urbana & Estabelecimentos** | [[SYS_PLAYER_INTERACTION]], [[SYS_INTERACTIVE_ESTABLISHMENTS]], [[SYS_INTERIOR_STREAMING]] | `IMPLEMENTADO_EM_FASE_5_12` | Contrato IInteractable, Detector Zero GC, Botão Touchscreen, Oficinas, Posto Pioneiro, Streaming Interiores |
| **SYS-27** | **Reconstrução Fiel de Araguari (IBGE Censo 2022)** | [[SYS_IBGE_FAITHFUL_RECONSTRUCTION]], [[ARAGUARI_GEO_AUDIT]], [[FASE_6_GEO_PILOT_01_COMPLETED]], [[GEO_PILOT_02_VALIDATION_REPORT]], [[GEO_PILOT_03_VALIDATION_REPORT]], [[GITHUB_MIGRATION_REPORT]], [[MARCO_A_COMPILATION_STABILIZATION_REPORT]] | `COMPILACAO_ESTABILIZADA` | Commit 38267d3, Cena AraguariPilotCentroScene (Scene 0), Titan 160 Real, 44 Quadras, 29 Vias, 21 Cruzamentos, Namespaces C# Validados, EditorBuildSettings Configurado |

---

## 4. Decisões Arquiteturais Vigentes (ADRs)

* [[ADR_001_UNITY6_URP]]: Adoção da Unity 6 (6000.3.6f1) e Universal Render Pipeline para alta performance mobile.
* [[ADR_002_CLEAN_ARCHITECTURE_PHYSICS]]: Separação estrita entre simulação física (`Rigidbody`/Torques) e renderização visual/animação.
* [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]: Utilização de `ScriptableObjects` para parâmetros de motos, peças e curvas de torque.
* [[ADR_004_CLEAN_ROOM_RECONSTRUCTION]]: Proibição de cópia de código proprietário do jogo original, implementando apenas contratos funcionais independentes.

---

## 5. Referências aos Documentos de Engenharia Reversa Existentes

Os laudos técnicos detalhados da análise estática residem em `03_TECHNICAL_REFERENCE/`:
* [[RELATORIO_IL2CPP_COMPLETO]] — Despejo do assembly ARM64, vtables e estruturas.
* [[MAPEAMENTO_FISICA_MOTOCICLETAS]] — Mapeamento dos 89 campos de `BikeWheelie`.
* [[FORMULAS_FISICA_MOTOS]] — Equações de torque de subida, amortecimento, potência e histerese.
* [[CATALOGO_ASSETS_COMPLETO]] — Inventário de motos, peças, avatares e áudios.
* [[PLANO_DE_TESTES_FISICOS]] — Casos de teste de aceleração, grau e frenagem.
* [[PENDENCIAS_E_INFORMACOES_NAO_CONFIRMADAS]] — Classificação de confiabilidade e lacunas.

---

## 6. Procedimento de Trabalho para Agentes de IA

1. **Entrada Obrigatória**: Ler [[AGENT_START_HERE]] antes de qualquer ação.
2. **Protocolo de Recuperação Seletiva**: Siga as 8 etapas de [[CONTEXT_RETRIEVAL_PROTOCOL]] (não leia arquivos desnecessários à tarefa).
3. **Padrões de Código e Documentação**: Siga [[CODING_STANDARDS]] e [[DOCUMENTATION_RULES]].
4. **Fechamento e Transferência**: Finalize toda sessão gerando o relatório obrigatório previsto em [[HANDOFF_PROTOCOL]].
5. **Mapeamento Relacional**: Consulte [[RELATIONSHIP_INDEX]] e [[GRAPHIFY_GUIDE]] para verificar dependências cruzadas e impactos colaterais.
