---
id: CHANGELOG
tipo: historico
projeto: Projeto Grau 2
---

# HISTÓRICO DE ALTERAÇÕES E VERSÕES (CHANGELOG)

## [Fase 6 — Marco A: Consolidação da Compilação e Estabilização do Projeto] — 2026-11-23
### Adicionado / Modificado
* Sincronização do Fix da Build #1: Integrada a correção remota de `using ProjetoGrau2.CameraControl;` em `MotorcycleHUD.cs` e o workflow de build `.github/workflows/android-dev.yml`.
* Auditoria Estática de C#: Varredura completa de 111 arquivos e 170 tipos C#. Identificado e corrigido preventivamente o uso de `AIVehicleSteeringController` em `PoliceDispatchManager.cs` através da adição de `using ProjetoGrau2.AI;`.
* Configuração da Cena Inicial (Build Settings): Criado `ProjectSettings/EditorBuildSettings.asset` registrando explicitamente `Assets/Scenes/AraguariPilotCentroScene.unity` como a cena de boot (enabled: 1) para Unity Cloud Build e exportadores nativos.
* Regressão Completa de Testes: 13/13 suítes aprovadas com 512 testes (100% de sucesso).
* Relatório Técnico: Criado `MARCO_A_COMPILATION_STABILIZATION_REPORT.md` e publicado no repositório no commit `38267d3`.

---

## [Fase 6 — Missão: Migração Oficial para o GitHub (Rery City)] — 2026-11-23
### Adicionado / Modificado
* Sincronização Segura do Repositório: Conectado e enviado o projeto `ProjetoGrau2_Unity` para o repositório privado `https://github.com/gusty22/ReryCity.git` na branch `main`.
* Preservação do Histórico e Estrutura: Preservados `README.md` e `.gitignore` preexistentes. A raiz do repositório foi vinculada diretamente à raiz do projeto Unity (sem subpastas artificiais).
* Auditoria Limpa: 323 arquivos versionados (1.16 MiB no total, zero arquivos > 50 MiB, zero arquivos de cache, segredos ou assets descompilados).
* Validação via GitHub API: Confirmada a presença remota de `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, cenas e 13 suítes de testes automatizados.
* Documentação de Migração: Produzido `GITHUB_MIGRATION_REPORT.md` detalhando auditoria, commits e proposta de compilação em nuvem (Game.ci / GitHub Actions).

---

## [Fase 6 — Missão GEO-PILOT-03: Homologação Unity, Preparação de Build Android e Testes Galaxy A54] — 2026-11-23
### Adicionado / Modificado
* Auditoria de Ambiente e Ferramentas: Diagnosticada presença de Android SDK (`C:\Users\washi\AppData\Local\Android\Sdk`), NDK 27, JDK 17 e ADB. Detectada ausência de Unity Editor / Unity Hub e estado crítico de espaço livre em disco no `C:\` (1.19 GB livres vs 24.69 GB no `E:\`).
* Risco do OneDrive Mapeado: Avaliado e documentado o risco severo de execução do Unity dentro do OneDrive (bloqueios de I/O em `Library/` e estouro de disco), recomendando workspace local dedicado fora do OneDrive.
* Correção Estrutural da Cena Piloto: Corrigida anomalia de hierarquia YAML em `AraguariPilotCentroScene.unity` onde transforms de rodas dianteira e traseira apontavam para a raiz da moto. Criados GameObjects filhos independentes `FrontWheelAnchor` (`fileID: 312`) e `RearWheelAnchor` (`fileID: 313`). A cena agora possui 12 GameObjects com 100% de conformidade.
* Registro Oficial da Versão: Criado `ProjectSettings/ProjectVersion.txt` configurando `m_EditorVersion: 6000.3.6f1`.
* Automação de Build Android: Atualizado `AndroidBuildAutomation.cs` com importação de `UnityEditor.Build` para conformidade estrita no Unity 6.
* Regressão e Validação: Suíte `test_araguari_faithful_reconstruction.py` expandida para 15 testes. Regressão geral de 13 suítes aprovada com 510 testes e 0 falhas em `run_all_tests.py`.
* Documentação de Homologação: Produzido `GEO_PILOT_03_VALIDATION_REPORT.md` respondendo detalhadamente às 12 perguntas oficiais da missão.

---

## [Fase 6 — Missão GEO-PILOT-01: Primeira Região Geográfica Real e Jogável] — 2026-11-23
### Adicionado
* Cena Piloto Real de Araguari: Construída `Assets/Scenes/AraguariPilotCentroScene.unity` (`GUID: 3000...06`) representando a região de 550x550m ao redor da Praça Manoel Bonito, com superfície contínua, iluminação solar, câmera de perseguição e infraestrutura de exploração.
* Integração da Titan 160 Real: Incorporada a montagem física completa da `Motorcycle_Titan160` com `Rigidbody`, `MotorcycleController`, `WheelieController`, motor, câmbio, `AthlonECUController`, áudio, suspensão, piloto humanoide (`Rider_Humanoid`) e detector de interação (`PlayerInteractionDetector`).
* Validação Poligonal e Corredores do IBGE: As 44 quadras reais do Centro foram validadas com polígonos fechados (área média de 13.228 m² e orientação CCW). Foram estruturados 29 corredores viários (distinguindo larguras medidas entre faces de larguras estimadas na borda) e 21 interseções conectadas.
* Suporte Android no Runtime: `AraguariRealCityBuilder.cs` atualizado com corrotina compatível com `UnityWebRequest` para leitura de StreamingAssets dentro de APKs Android e fallback via `File.ReadAllText` no Desktop.
* Regressão e Validação: Suíte `test_araguari_faithful_reconstruction.py` expandida para 11 testes. Regressão geral de 13 suítes aprovada com 506 testes e 0 falhas em `run_all_tests.py`.
* Documentação e Grafo: Criado `FASE_6_GEO_PILOT_01_COMPLETED.md` e sincronizado o grafo de conhecimento.

---

## [Fase 6 — Reconstrução Geográfica Fiel de Araguari (MG) — Fase Inicial] — 2026-11-23
### Adicionado
* Auditoria Completa de Fidelidade Geográfica: Criado `ARAGUARI_GEO_AUDIT.md` avaliando as 10 dimensões cartográficas do projeto atual vs a geografia real de Araguari.
* Ingestão e Processamento do Dataset IBGE Censo 2022: Implementado pipeline offline em Python `Tools/GeoPipeline/process_araguari_ibge.py` processando 8.430 faces de quadras, 1.194 logradouros e 1.946 quadras do arquivo oficial `3103504_faces_de_logradouros_2022.json`.
* Região Piloto do Centro de Araguari: Extraída área piloto de ~550m × 550m ao redor do Marco Zero da Praça Manoel Bonito (`-18.649100° S, -48.190300° O`), gerando `Assets/StreamingAssets/Geography/araguari_pilot_geometry.json` com 148 faces de quadra, 44 quarteirões poligonais fechados com dimensões reais e 30 vias oficiais (Av. Teodolino Pereira Araújo, Av. Tiradentes, Praça Manoel Bonito, Rua Marciano Santos, Rua Rui Barbosa, Rua Brasil Accioly, etc.).
* Estruturas C# e Construtor de Cidade Real: Implementados `RealCityDataPayload.cs` (`GUID: 1000...c3`), `AraguariRealCityBuilder.cs` (`GUID: 1000...c4`) e `RealRoadCorridor.cs` (`GUID: 1000...c5`) para consumo 100% offline dos dados geoespaciais em runtime.
* Suíte de Testes Automatizados: Criado `test_araguari_faithful_reconstruction.py` (9 testes) e atualizado `run_all_tests.py` para 13 suítes completas com 100% de aprovação.
* Documentação e Grafo: Criado `SYS_IBGE_FAITHFUL_RECONSTRUCTION.md` e sincronizado Obsidian Vault.

---

## [Fase 5 — Etapa 5.12: RERY CITY — Vida Urbana, Estabelecimentos Interativos e Sistema de Interação] — 2026-11-23
### Adicionado
* Sistema Central de Interação: Implementados o contrato `IInteractable.cs` (`GUID: 1000...bc`) e o componente de detecção não-alocativo `PlayerInteractionDetector.cs` (`GUID: 1000...bd`), com buffer não-alocativo (`Physics.OverlapSphereNonAlloc`), seleção do candidato mais próximo, bloqueio de controles veiculares durante interação e suporte a toque mobile e teclado no PC.
* Interface Contextual Touchscreen: Implementado `InteractionContextUI.cs` (`GUID: 1000...be`) com botão mobile tátil e prompt dinâmico animado via `CanvasGroup.alpha`.
* Estabelecimentos Comerciais e Serviços: Implementados `InteractiveEstablishmentConfig.cs` (`GUID: 1000...c2`), `InteractiveEstablishment.cs` (`GUID: 1000...bf`) e asset `InteractiveEstablishment_Default.asset` (`GUID: 2000...ae`) para suporte a oficinas, postos, borracharias e mercadinhos com débito de moeda diretamente no `SaveSystem` e estacionamento com alinhamento e anulação de inércia física da moto.
* Bomba de Combustível Interativa: Implementado `GasStationPump.cs` (`GUID: 1000...c0`) no "Posto Pioneiro", com litragem adaptativa, preço por litro configurável, abastecimento parcial se o saldo for menor e prevenção de transbordamento.
* Streaming Leve de Interiores com Histerese: Implementado `EstablishmentInteriorManager.cs` (`GUID: 1000...c1`), ativando e desativando objetos internos e luzes por proximidade (20m / 28m) sem alocações de GC e sem telas de carregamento artificiais.
* Integração na Cena: Adicionados `PlayerInteractionDetector` na moto (`Player_Motorcycle_Proxy`), `InteractionContextUI` no HUD (`Main_Player_HUD_Canvas`) e o nó `Araguari_Interactive_Establishments` em `UrbanTrafficTestScene.unity`.
* Testes Automatizados e Regressão Completa: Criado `test_urban_interactions.py` (13 testes) e atualizado `run_all_tests.py`, totalizando 495 testes aprovados com 0 falhas em 12 suítes completas (Fases 5.1 a 5.12).
* Documentação técnica criada: `SYS_PLAYER_INTERACTION.md`, `SYS_INTERACTIVE_ESTABLISHMENTS.md`, `SYS_INTERIOR_STREAMING.md` e `07_TASKS/FASE_5_12_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.11: RERY CITY — Ambientação Urbana e Detalhamento de Araguari (MG)] — 2026-11-23
### Adicionado
* Temática Diferenciada dos 7 Bairros: Implementado `NeighborhoodThemeCatalog.cs` (`GUID: 1000...b8`) e asset `NeighborhoodThemes_Default.asset` (`GUID: 2000...ac`) definindo densidade, arborização, calçamento e vocação para Centro, Bosque, Goiás, Miranda, Amorim, Santa Helena e Sibipiruna.
* Marcas Comerciais Fictícias: Implementado `CommercialSignboardCatalog.cs` (`GUID: 1000...bb`) e asset `CommercialSignboards_Default.asset` (`GUID: 2000...ad`) contendo 6 marcas fictícias autênticas ("Oficina do Grau", "Borracharia do Baiano", "Mercadinho Central", etc.).
* Detalhador de Fachadas: Implementado `FacadeDetailGenerator.cs` (`GUID: 1000...b9`) gerando muros de alvenaria com vão de portão (3.5m) e letreiros comerciais horizontais respeitando os recuos dos lotes.
* Detalhador de Vias e Calçadas: Implementado `StreetInfrastructureDetailer.cs` (`GUID: 1000...ba`) posicionando meio-fio, lombadas suaves (10cm) para motos e faixas de pedestres.
* Integração na Cena: Adicionado nó `Araguari_City_Detail_Root` em `UrbanTrafficTestScene.unity`.
* Testes Automatizados e Regressão: Criado `test_city_environment_detail.py` (11 testes) e atualizado `run_all_tests.py`, totalizando 482 testes aprovados com 0 falhas em 11 suítes completas (Fases 5.1 a 5.11).
* Documentação técnica criada: `SYS_NEIGHBORHOOD_IDENTITY.md`, `SYS_FACADE_DETAILS.md`, `SYS_STREET_INFRASTRUCTURE.md` e `07_TASKS/FASE_5_11_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.10: RERY CITY — Construção Visual e Arquitetônica de Araguari (MG)] — 2026-11-23
### Adicionado
* Sistema de Lotes e Edifícios Modulares: Implementados `UrbanLot.cs` (`GUID: 1000...b2`), `BuildingGenerator.cs` (`GUID: 1000...b3`), `BuildingArchetypeCatalog.cs` (`GUID: 1000...b7`) e asset `BuildingArchetypeCatalog_Default.asset` (`GUID: 2000...ab`), com controle de recuo frontal obrigatório para evitar sobreposição com as vias de Araguari e geração volumétrica determinística.
* Biblioteca Central de Materiais: Implementado `CityMaterialLibrary.cs` (`GUID: 1000...b4`) e asset `CityMaterialLibrary_Default.asset` (`GUID: 2000...aa`), com 11 categorias de materiais otimizados para URP e GPU Instancing ativado para contenção de Draw Calls no Android.
* Mobiliário Urbano e Iluminação Pública: Implementados `UrbanFurnitureManager.cs` (`GUID: 1000...b5`) e `UrbanLightingController.cs` (`GUID: 1000...b6`), com distribuição de postes e placas ao longo das calçadas, ciclo dia/noite e adaptação automática aos perfis de energia mobile (Econômico, Equilibrado e Alto Desempenho).
* Integração na Cena: Adicionado nó `Araguari_Visual_Architecture` em `UrbanTrafficTestScene.unity`.
* Testes Automatizados e Regressão: Criado `test_city_visual_infrastructure.py` (11 testes) e atualizado `run_all_tests.py`, totalizando 471 testes aprovados com 0 falhas em 10 suítes completas (Fases 5.1 a 5.10).
* Documentação técnica criada: `SYS_URBAN_LOTS_BUILDINGS.md`, `SYS_CITY_MATERIALS.md`, `SYS_URBAN_FURNITURE_LIGHTING.md` e `07_TASKS/FASE_5_10_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.9: RERY CITY — Construção da Cidade Real de Araguari (MG)] — 2026-11-23
### Adicionado
* Sistema de Conversão Geográfica: Implementado `GeoCoordinateConverter.cs` (`GUID: 1000...ac`) e `GeoOriginConfig.cs` (`GUID: 1000...ad`), provendo conversão bidirecional entre coordenadas WGS84 e o espaço vetorial Unity com precisão submilimétrica.
* Origem Cartográfica: Criado `GeoOrigin_AraguariCentro.asset` (`GUID: 2000...a8`) fixando o Marco Zero na Praça Manoel Bonito (-18.649100° S, -48.190300° O, 970m alt).
* Catálogo Urbano e Censitário: Implementado `AraguariCityDataCatalog.cs` (`GUID: 1000...b1`) e asset `AraguariCityCatalog_Default.asset` (`GUID: 2000...a9`) com 7 bairros oficiais (Centro, Bosque, Goiás, Miranda, Amorim, Santa Helena, Sibipiruna), dados de licenciamento OpenStreetMap/ODbL/IBGE, vias e POIs notáveis.
* Malha Viária Real e Indexador: Implementados `RealRoadSegment.cs` (`GUID: 1000...af`) e `RealRoadNetworkManager.cs` (`GUID: 1000...b0`) com indexação espacial por células de streaming (`WorldChunkCoord`), busca de via mais próxima e integração ao `TrafficNavigationManager`.
* Pontos de Interesse: Implementado `GeoPointOfInterest.cs` (`GUID: 1000...ae`) com sincronização de coordenadas reais.
* Integração na Cena: Adicionado nó `Araguari_City_Infrastructure` em `UrbanTrafficTestScene.unity` com `RealRoadNetworkManager`, `POI_PracaManoelBonito` e `Road_Av_Theodolino`.
* Testes e Regressão: Criado `test_araguari_world.py` (11 testes) e atualizado `run_all_tests.py`, atingindo 460 testes aprovados com 0 falhas em 9 suítes completas (Fases 5.1 a 5.9).
* Documentação técnica criada: `SYS_ARAGUARI_GEOGRAPHY.md`, `SYS_GEOGRAPHIC_COORDINATES.md`, `SYS_REAL_ROAD_NETWORK.md`, `SYS_MAP_STREAMING_INTEGRATION.md`, `SYS_POINT_OF_INTEREST.md` e `07_TASKS/FASE_5_9_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.8: RERY CITY — Transformação em Jogo Independente] — 2026-11-23
### Adicionado
* Identidade Centralizada: Implementado `ReryCityIdentityConfig.cs` (`GUID: 1000...a6`) e asset `ReryCityIdentity_Default.asset` (`GUID: 2000...a6`), definindo o nome oficial do produto como **RERY CITY**, ambientação em Araguari Futurista, desvinculação pública do título legado e flags de modo 100% offline.
* Inicialização Desacoplada: Implementado `OfflineGameBootstrapper.cs` (`GUID: 1000...a7`) gerenciando máquina de 5 estados para inicialização sem bloqueios de rede.
* Recompensas Simuladas Offline: Implementado `SimulatedRewardManager.cs` (`GUID: 1000...a8`) com proteção de debouncing, cooldown de 15s e concessão de moedas locais com persistência no `SaveSystem`, eliminando dependências de SDKs publicitários externos.
* Carregamento Assíncrono Real: Implementado `AsynchronousSceneLoader.cs` (`GUID: 1000...a9`) com normalização da faixa nativa da Unity e interpolação suave de progresso.
* Perfis de Energia e Desempenho Mobile: Implementados `MobileEnergyProfileManager.cs` (`GUID: 1000...aa`), `MobileEnergyProfileConfig.cs` (`GUID: 1000...ab`) e asset `MobileEnergyProfile_Default.asset` (`GUID: 2000...a7`), fornecendo os modos Econômico (30 FPS), Equilibrado (60 FPS) e Alto Desempenho (60 FPS) com persistência em PlayerPrefs.
* Integração na Cena: Adicionado GameObject `ReryCity_GameManager` em `UrbanTrafficTestScene.unity` agregando os 4 controladores.
* Validação e Runner: Criado `test_rery_city_offline.py` (16 testes) e o executor unificado `run_all_tests.py`, corrigindo o comportamento de importação do `unittest` e atingindo 449/449 testes aprovados (100%) em todas as 8 suítes das Fases 5.1 a 5.8.
* Documentação técnica criada: `SYS_GAME_IDENTITY.md`, `SYS_OFFLINE_INITIALIZATION.md`, `SYS_SIMULATED_REWARDS.md`, `SYS_GAME_LOADING.md`, `SYS_ENERGY_MANAGEMENT.md` e `07_TASKS/FASE_5_8_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.7: Construção do Mundo Urbano e Ambiente de Jogo] — 2026-11-23
### Adicionado
* Scripts de Streaming e Células: `WorldChunkCoord.cs`, `WorldChunk.cs` e `WorldStreamer.cs` (`GUID: 1000...9c` a `9e`).
* Scripts de Geração Procedural: `ProceduralBlockConfig.cs` e `ProceduralEnvironmentGenerator.cs` (`GUID: 1000...9f` e `a0`).
* ScriptableObjects em `Assets/ScriptableObjects/World/`: `ProceduralBlockConfig_Residential.asset` e `ProceduralBlockConfig_Commercial.asset` (`GUID: 2000...a0` e `a1`).
* Scripts de População e Pedestres: `PedestrianController.cs`, `PedestrianPool.cs` e `WorldPopulationManager.cs` (`GUID: 1000...a1` a `a3`).
* Scripts de LOD e Otimização: `WorldLODGroup.cs` e `WorldOptimizationManager.cs` (`GUID: 1000...a4` e `a5`).
* Cena `UrbanTrafficTestScene.unity` atualizada com o nó `World_Infrastructure_Root` contendo todos os subsistemas de streaming, geração e otimização.
* Bateria de testes automatizados `test_world_infrastructure.py` com 72/72 asserções aprovadas (100%) e regressão completa das Fases 5.1 a 5.7 totalizando 433 testes aprovados com 0 falhas.
* Documentação técnica criada: `SYS_WORLD_STREAMING.md`, `SYS_PROCEDURAL_ENVIRONMENT.md`, `SYS_WORLD_POPULATION.md`, `SYS_WORLD_OPTIMIZATION.md` e `07_TASKS/FASE_5_7_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.6: Integração Geral, HUD e Experiência do Jogador] — 2026-11-23
### Adicionado
* Scripts de Interface e Áudio: `MotorcycleHUD.cs` (`GUID: 1000...98`), `WorldEventNotificationUI.cs` (`GUID: 1000...99`), `TouchscreenMobileControlsUI.cs` (`GUID: 1000...9a`) e `AudioIntegrationManager.cs` (`GUID: 1000...9b`).
* Cena `UrbanTrafficTestScene.unity` atualizada com o Canvas `Main_Player_HUD_Canvas` (1920x1080 com CanvasScaler) e entidade `AudioIntegrationManager`.
* Bateria de testes automatizados `test_ui_integration.py` com 48/48 asserções aprovadas (100%) e regressão completa das Fases 5.1, 5.2, 5.3, 5.4, 5.5 e 5.6 totalizando 361 testes aprovados com 0 falhas.
* Documentação técnica criada: `SYS_MOTORCYCLE_HUD.md`, `SYS_MOBILE_CONTROLS.md`, `SYS_AUDIO_INTEGRATION.md` e `07_TASKS/FASE_5_6_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.5: Integração Avançada de Perseguições Policiais] — 2026-11-23
### Adicionado
* Scripts de Coordenação e Papéis Táticos: `PoliceTacticalRole.cs` e `PolicePursuitCoordinator.cs` (`GUID: 1000...96` e `97`).
* Evolução em `PolicePursuitController.cs`: Registro dinâmico no coordenador, compartilhamento de rádio COPOM (LKP global), cálculo de posições táticas (Lead, Flanker, Interceptor) e busca em quadrantes.
* Cena `UrbanTrafficTestScene.unity` atualizada com o GameObject e MonoBehaviour `PolicePursuitCoordinator`.
* Bateria de testes automatizados `test_police_pursuit_advanced.py` com 37/37 asserções aprovadas (100%) e regressão completa das Fases 5.1, 5.2, 5.3, 5.4 e 5.5 totalizando 313 testes aprovados com 0 falhas.
* Documentação técnica criada: `SYS_PURSUIT_COORDINATION.md` e `07_TASKS/FASE_5_5_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.4: Ocorrências Dinâmicas, Bloqueios Viários e Eventos do Mundo] — 2026-11-23
### Adicionado
* Scripts de Eventos e Bloqueios: `WorldEventType.cs`, `WorldEventConfig.cs`, `WorldEventInstance.cs`, `RoadBlockage.cs` e `WorldEventManager.cs` (`GUID: 1000...90` a `94`).
* Script de Despacho Policial: `PoliceDispatchManager.cs` (`GUID: 1000...95`).
* ScriptableObjects em `Assets/ScriptableObjects/WorldEvents/`: `EventConfig_TrafficAccident.asset`, `EventConfig_RoadworkBlockage.asset`, `EventConfig_BrokenVehicle.asset` e `EventConfig_PoliceCheckpoint.asset` (`GUID: 2000...90` a `93`).
* Modificações cirúrgicas em `TrafficLane.cs` (`isBlocked`) e `AIVehicleController.cs` (desvio tático autônomo com detecção de interdição).
* Cena `UrbanTrafficTestScene.unity` atualizada com entidades de `WorldEventManager`, `PoliceDispatchManager` e instância demonstrativa `WorldEvent_Roadwork_Demo` com bloqueio e cones físicos.
* Bateria de testes automatizados `test_world_events.py` com 72/72 asserções aprovadas (100%) e regressão completa das Fases 5.1, 5.2 e 5.3 com 276 testes totais aprovados sem falhas.
* Documentação técnica criada: `SYS_WORLD_EVENTS.md`, `SYS_ROAD_BLOCKAGE.md`, `SYS_POLICE_DISPATCH.md` e `07_TASKS/FASE_5_4_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.3: Inteligência Artificial Policial e Sistema de Procurado] — 2026-11-23
### Adicionado
* Scripts de Dados Policiais: `PoliceVehicleData.cs` e `PoliceAIData.cs` (`GUID: 1000...71` e `72`).
* Máquina de Estados e Perseguição: `PoliceAIStateMachine.cs` (8 estados) e `PolicePursuitController.cs` (`GUID: 1000...73` e `76`).
* Sistema de Infrações e Procurado: `TrafficViolationDetector.cs` (5 infrações com cooldown) e `WantedLevelManager.cs` (0 a 5 estrelas e evasão) (`GUID: 1000...74` e `75`).
* Interface e Feedback: `PoliceHUD.cs` e `WantedLevelUI.cs` (`GUID: 1000...77` e `78`).
* ScriptableObjects em `Assets/ScriptableObjects/Police/`: `PoliceVehicleData_ViaturaHilux.asset`, `PoliceVehicleData_RocamTiger800.asset`, `PoliceAIData_StandardPatrol.asset` e `PoliceAIData_RocamPursuit.asset` (`GUID: 2000...80` a `83`).
* Cena `UrbanTrafficTestScene.unity` atualizada com instâncias ativas de viatura Hilux, moto ROCAM, `TrafficViolationDetector`, `WantedLevelManager` e canvas de interface.
* Bateria de testes automatizados `test_police_ai.py` com 68/68 asserções aprovadas (100%).
* Documentação técnica criada: `SYS_POLICE_AI.md`, `SYS_POLICE_PURSUIT.md`, `SYS_TRAFFIC_VIOLATIONS.md`, `SYS_WANTED_LEVEL.md`, `SYS_POLICE_HUD.md` e `07_TASKS/FASE_5_3_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.2: Veículos Civis NPC, Física Veicular e Inteligência de Tráfego] — 2026-11-23
### Adicionado
* Scripts de Dados de IA: `AIVehicleType.cs`, `AIDriverProfile.cs` e `AIVehicleData.cs`.
* Scripts Controladores de IA: `AIVehicleStateMachine.cs` (10 estados e histerese), `AIVehicleSensor.cs` (Zero GC em 360°), `AIVehicleMotorController.cs` (propulsão física longitudinal), `AIVehicleSteeringController.cs` (Pure Pursuit e lean de moto), `AIVehicleBrakeController.cs` (frenagem física e trava anti-recuo) e `AIVehicleController.cs` (orquestrador mestre).
* Scripts de Gerenciamento e Otimização: `AIVehiclePool.cs` (pooling com instâncias proxy), `TrafficLODManager.cs` (LOD comportamental time-sliced) e `TrafficSpawnManager.cs` (densidade perimetral inteligente).
* ScriptableObjects em `Assets/ScriptableObjects/Traffic/`: 3 perfis de motoristas (Cauteloso, Normal, Agressivo) e 8 categorias de veículos (CivilMotorcycle, EconomyCar, Sedan, SUV, Pickup, Truck, Bus, PoliceCruiser).
* Cena `UrbanTrafficTestScene.unity` atualizada com entidades de pool, LOD, spawner, veículo líder, obstáculo e proxy da moto do jogador.
* Bateria de testes automatizados `test_ai_vehicles.py` com 93/93 asserções aprovadas.
* Documentação técnica criada: `SYS_AI_VEHICLES.md`, `SYS_AI_DRIVER.md`, `SYS_AI_SENSORS.md`, `SYS_TRAFFIC_SPAWNING.md`, `SYS_TRAFFIC_OPTIMIZATION.md` e `07_TASKS/ETAPA_5_2_COMPLETED.md`.

---

## [Fase 5 — Etapa 5.1: Infraestrutura de Navegação Urbana] — 2026-11-23
### Adicionado
* `TrafficWaypoint.cs`: Ponto geométrico vetorial com velocidade regulamentada, linhas de retenção e associação semafórica.
* `TrafficLane.cs`: Faixa de rolamento com ordenação de waypoints, faixas adjacentes para ultrapassagens e consultas espaciais.
* `TrafficNode.cs`: Nós topológicos de bifurcação, confluência e entroncamento com seleção probabilística ponderada de rotas para NPCs.
* `TrafficSignal.cs`: Semáforo urbano com ciclo de 4 fases (Green, Yellow, AllRed de segurança e Red).
* `TrafficIntersection.cs`: Gerenciador de cruzamento urbano com coordenação semafórica, regras de prioridade e controle de ocupação anti-bloqueio.
* `TrafficWaypointNetwork.cs`: Grafo contêiner da malha viária com validação topológica automática.
* `TrafficNavigationManager.cs`: Singleton central para consultas espaciais, verificação de prioridades e renderização de Gizmos coloridos no editor.
* Cena `UrbanTrafficTestScene.unity` e respectivos arquivos `.meta` com GUIDs determinísticos padronizados (`1000...40` a `46`, `3000...05`).
* Bateria de testes automatizados `test_traffic_navigation.py` aprovada com 43 asserções.
* Documentação técnica criada: `SYS_TRAFFIC_NAVIGATION.md` e `07_TASKS/ETAPA_5_1_COMPLETED.md`.

---

## [Fase 4: Piloto Humanoide, IK, Animações, Ragdoll e Equipamentos] — 2026-11-23
### Adicionado
* `RiderData.cs`: ScriptableObject com biometria e limiares posturais.
* `MotorcycleRiderController.cs`: Orquestrador de estados corporais (Idle, Riding, Accelerating, Braking, WheelieBalanced, WheelieRaspa, CrashedRagdoll).
* `RiderIKController.cs`: Cinemática Inversa nativa com `OnAnimatorIK` (mãos, pés, look-at) sem alocação no heap.
* `RiderPoseController.cs`: Modulação procedural de coluna, tórax e cabeça para inclinação em curvas (*lean banking*), arrasto aerodinâmico e raspão no grau.
* `RiderRagdollController.cs`: Transição de Animator para corpos rígidos em capotamentos e colisões, com ejeção física e recuperação via tecla reset.
* `RiderEquipmentData.cs` e `RiderEquipmentManager.cs`: Sistema de vestimentas, capacetes e acessórios modulares nos ossos âncora.
* Ativo `DefaultRider_Data.asset` e hierarquia `Rider_Humanoid` vinculada ao chassi da moto em `PhysicsTestScene.unity`.
* Documentação técnica criada no Obsidian: `SYS_RIDER.md`, `SYS_RIDER_IK.md`, `SYS_RIDER_ANIMATIONS.md`, `SYS_RIDER_RAGDOLL.md`, `SYS_RIDER_EQUIPMENT.md` e `07_TASKS/FASE_4_COMPLETED.md`.

---

## [Fase 3: Oficina, Customização Mecânica, Câmeras e Interface Athlon] — 2026-11-23
### Adicionado
* `MotorcycleCameraController.cs`: Sistema de câmeras em 3 modos (3ª pessoa orbital anti-clipping, 1ª pessoa no capacete com lean banking, e câmera de ação para o grau).
* `SuspensionPreset.cs` e `SuspensionController.cs`: Sistema de molejo ativo dianteiro e traseiro massa-mola-amortecedor calculada em FixedUpdate.
* `PartItemData.cs` e `WorkshopManager.cs`: Catálogo modular de peças da oficina e inspeção em 360°.
* `AthlonMobileApp.cs`: Smartphone in-game com interface funcional para limitador de RPM, Two-Step, histerese, corte de ignição/injeção e perfis salvos.
* `SaveSystem.cs`: Sistema de persistência JSON multiplataforma (`Application.persistentDataPath`).
* Ativo `Titan160_SuspensionPreset.asset` e atualização de `PhysicsTestScene.unity`.
* Documentação técnica criada no Obsidian: `SYS_CAMERA.md`, `SYS_WORKSHOP.md`, `SYS_CUSTOMIZATION.md`, `SYS_ATHLON_MOBILE.md`, `SYS_SAVE_SYSTEM.md` e `07_TASKS/FASE_3_COMPLETED.md`.

---

## [Fase 2: Sistema de Propulsão, Motor e Áudio] — 2026-11-23
### Adicionado
* `EngineData.cs` e `EngineController.cs`: Cálculo de RPM, marcha lenta, inércia do virabrequim, freio-motor e curvas de torque e potência ($P_{CV} = (\tau \cdot RPM)/7023.5$).
* `TransmissionData.cs` e `TransmissionController.cs`: Caixa de câmbio de 5 marchas, reduções primária e final, embreagem gradual, gear drop físico e força tratora calculada.
* `AthlonECUData.cs` e `AthlonECUController.cs`: Limitador de giro com histerese (Schmitt Trigger), controle de largada Two-Step (5200 RPM) e gerador de estampidos/pops.
* `MotorAudioProfile.cs` e `MotorAudioController.cs`: Sistema de áudio reativo desacoplado da física, pitch contínuo e disparos pontuais (one-shot).
* Ativos `Titan160_EngineConfig.asset`, `Titan160_TransmissionConfig.asset`, `Titan160_AthlonConfig.asset` e `Titan160_AudioProfile.asset`.
* Documentação técnica criada no Obsidian: `SYS_ENGINE.md`, `SYS_TRANSMISSION.md`, `SYS_ATHLON.md`, `SYS_AUDIO.md` e `07_TASKS/FASE_2_COMPLETED.md`.

---

## [Fase 1: Núcleo Físico do Grau] — 2026-11-23
### Adicionado
* Configuração do projeto Unity 6 em `C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity` (URP 17.0.3, Input System 1.8.2, Cinemachine, Mathematics).
* `ProjectSettings/DynamicsManager.asset`: Fixed Timestep calibrado a 60 Hz (0.0166667s), gravidade -9.81 m/s², solver de 8 iterações.
* `MotorcycleData.cs`, `IMotorcycleInput.cs`, `CrossPlatformInputHandler.cs`.
* `WheeliePhysicsController.cs`: Equações diferenciais de momento e torque angular de arfagem (Pitch) em torno do eixo X.
* `MotorcycleController.cs`: Locomoção longitudinal, esterçamento restrito em grau (`noTurnAngle = 52°`), downforce e recuperação.
* `PhysicsTelemetryUI.cs`: HUD de telemetria física visual em tempo real em `OnGUI` nativo.
* `Titan160_Data.asset` e cena de teste `PhysicsTestScene.unity`.
* Auditoria técnica formal e correções cirúrgicas registradas em `05_TESTING/FASE_1_PHYSICS_AUDIT.md`.

---

## [Fase 0: Fundação de Engenharia e Gestão de Conhecimento] — 2026-11-23
### Adicionado
* **Estrutura do Vault Obsidian**: Cofre em `C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Knowledge`.
* **Índice Central e Protocolos**: `PROJECT_INDEX.md`, `AGENT_START_HERE.md`, `CONTEXT_RETRIEVAL_PROTOCOL.md`, `HANDOFF_PROTOCOL.md`, `CODING_STANDARDS.md`, `DOCUMENTATION_RULES.md`.
* **Decisões Arquiteturais (ADRs)**: `ADR_001` (Unity 6 / URP), `ADR_002` (Física desacoplada), `ADR_003` (ScriptableObjects), `ADR_004` (Clean-Room).
* **Motor Relacional de Conhecimento (Graphify Engine)**: `10_GRAPH/build_graph.py` e suíte de testes `10_GRAPH/test_knowledge_system.py`.
