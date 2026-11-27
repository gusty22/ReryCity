# RERY CITY (`com.InsanityGroup.ProjetoGrau2`)

> **Jogo independente de motocicletas em mundo aberto na Unity 6, ambientado na geografia real de Araguari–MG e otimizado prioritariamente para o Samsung Galaxy A54 5G.**

---

## 1. Visão Geral

O **RERY CITY** é um jogo de mundo aberto e simulação física de motocicletas desenvolvido com **Unity 6 (`6000.3.6f1`)**, **Universal Render Pipeline (URP)** e **PhysX**. O projeto é construído segundo uma rigorosa metodologia **Clean-Room**, com arquitetura desacoplada, funcionamento 100% offline e foco em alto desempenho mobile.

A ambientação urbana reproduz com fidelidade topológica a malha viária do município de **Araguari, Minas Gerais, Brasil**, a partir de dados geodésicos oficiais do **IBGE (Censo 2022)**.

---

## 2. Pilares Arquiteturais (ADRs)

* **ADR-001 (Unity 6 + URP)**: Renderização moderna via Universal Render Pipeline, shaders customizados de baixo custo de amostragem e perfis de iluminação escaláveis para Android.
* **ADR-002 (Separação Físico-Visual)**: Simulação de `Rigidbody` e torques executada exclusivamente no passo fixo (`FixedUpdate`), desacoplada da apresentação e animação visual.
* **ADR-003 (Parametrização via ScriptableObjects)**: Curvas de torque, relações de marcha, mapas de injeção eletrônica (ECU Athlon), suspensão e peças configuradas em assets desacoplados.
* **ADR-004 (Metodologia Clean-Room)**: Implementação 100% original a partir de contratos e fórmulas físicas, sem inclusão de assets ou binários proprietários de terceiros.

---

## 3. Subsistemas Implementados

### 🏍️ Física Veicular & Propulsão (Titan 160)
* **Motor 162.7 cc**: Curvas realistas de torque (1.40 kgf.m @ 6000 RPM, 14.9 cv @ 8000 RPM, corte a 9500 RPM).
* **Transmissão**: Caixa sequencial de 5 marchas, embreagem e relações primária/secundária reais.
* **Física de Grau (Wheelie)**: Torque de elevação progressivo, equilíbrio angular, alívio de potência e freio traseiro de segurança.
* **Suspensão**: Garfo telescópico dianteiro e bi-amortecedor traseiro com anti-squat e detecção precisa de solo.
* **Injeção Eletrônica Athlon**: Simulação de ECU de competição com two-step, corte de giro e telemetria.
* **Áudio Granular**: Síntese sonora procedural de escape, aceleração e corte de ignição baseada em RPM.

### 🗺️ Geografia Real de Araguari (IBGE Censo 2022)
* **Marco Zero (Praça Manoel Bonito)**: Projeção cartográfica local WGS84 (`-18.649100° S, -48.190300° O`, 970m de altitude).
* **Malha Topológica**: 44 quadras poligonais fechadas (algoritmo Shapely `polygonize`), 29 corredores viários e 21 interseções conectadas.
* **Região Piloto Centro**: Setor navegável contínuo de 550m × 550m com leito asfáltico contínuo e colisão física.
* **Arquitetura Procedural**: Lotes urbanos, arquétipos de edifícios, fachadas, muros, calçadas e postes de iluminação URP.

### 🚗 Tráfego Civil & Inteligência Policial
* **Navegação Viária**: Grafo de faixas, regras de preferência, cruzamentos e cálculo de rotas.
* **Personalidades de IA**: Perfis de motoristas (Normal, Cauteloso, Agressivo) com cones de sensores e frenagem evasiva.
* **Polícia Ostensiva (ROCAM & Hilux)**: Patrulhamento urbano, despacho dinâmico de ocorrências e perseguição ativa baseada em nível de procurado.
* **Infrações de Trânsito**: Detecção de manobras de grau em vias públicas, excesso de velocidade e colisões.

### 📱 Experiência Mobile (Samsung Galaxy A54 5G)
* **Alvo de Desempenho**: Meta primária de 30 FPS estáveis; meta secundária de 60 FPS no perfil Alto Desempenho.
* **Controles Touchscreen**: Volante/setas direcionais, acelerador analógico, freios dianteiro e traseiro independentes, chave de grau e reset de queda.
* **HUD Zero-GC**: Telemetria em tempo real com dirty-checking para eliminar pausas de Garbage Collection.
* **Gerenciamento de Energia**: Perfis de bateria (Econômico, Equilibrado, Desempenho) ajustando sombras, LOD e resolução de renderização.

---

## 4. Estrutura do Repositório

```text
ReryCity/
├── .github/workflows/          # Workflows de CI/CD (compilação manual Android via GameCI)
├── Assets/
│   ├── Editor/                 # Automação de build Android (AndroidBuildAutomation.cs)
│   ├── Scenes/                 # Cenas do jogo (AraguariPilotCentroScene.unity - Cena 0)
│   ├── ScriptableObjects/      # Presets de motos, motores, suspensão, tráfego e IA
│   ├── Scripts/                # 111 classes e scripts C# distribuídos em 19 módulos
│   └── StreamingAssets/        # Dados geográficos offline (araguari_pilot_geometry.json)
├── Docs/
│   ├── Architecture/           # ADRs (ADR-001 a ADR-004) e visão de arquitetura
│   ├── Protocols/              # Guias para agentes, padrões de código e índice geral
│   ├── Reports/                # Relatórios técnicos das Fases 1 a 5 e Geo-Pilot 01 a 03
│   └── Systems/                # Especificações detalhadas dos subsistemas
├── Packages/                   # Dependências oficiais da Unity (URP 17, Input System, Cinemachine)
├── ProjectSettings/            # Configurações do projeto (EditorBuildSettings, DynamicsManager, ProjectVersion)
├── Tools/
│   ├── GeoPipeline/            # Ingestão cartográfica IBGE e gerador procedural Python
│   └── Graphify/               # Motor de análise de grafo de conhecimento e dependências
├── run_all_tests.py            # Orquestrador da suíte completa de testes
└── test_*.py                   # 12 suítes de testes unitários e de integração
```

---

## 5. Testes Automatizados

O repositório conta com **512 testes automatizados** cobrindo física, navegação, IA policial, streaming, reconstrução topológica e configurações de compilação:

```bash
# Executar a regressão completa (13 suítes):
python run_all_tests.py
```

### Suítes Incluídas:
1. `test_traffic_navigation.py` — Grafo viário e regras de cruzamento
2. `test_ai_vehicles.py` — Físicas e estados dos veículos civis
3. `test_police_ai.py` — Estados da polícia e níveis de infração
4. `test_world_events.py` — Ocorrências, acidentes e bloqueios
5. `test_police_pursuit_advanced.py` — Perseguições policiais avançadas
6. `test_ui_integration.py` — HUD, botões touch e zero-GC
7. `test_world_infrastructure.py` — LOD e streaming de mundo
8. `test_rery_city_offline.py` — Inicialização offline e persistência
9. `test_araguari_world.py` — Coordenadas WGS84 e malha de Araguari
10. `test_city_visual_infrastructure.py` — Geração visual e materiais
11. `test_city_environment_detail.py` — Bairros, calçadas e mobiliário
12. `test_urban_interactions.py` — Estabelecimentos interativos
13. `test_araguari_faithful_reconstruction.py` — Topologia IBGE e build settings

---

## 6. Como Compilar para Android

### Opção 1: Unity Cloud Build Automation
* **Projeto**: `RERY CITY`
* **Destino**: `RERY CITY - Android A54`
* **Repositório**: `gusty22/ReryCity`
* **Branch**: `main`
* A cena inicial está formalmente configurada em `ProjectSettings/EditorBuildSettings.asset` como `AraguariPilotCentroScene.unity`.

### Opção 2: Linha de Comando (Batchmode Local)
```powershell
& "<Caminho_Unity>\Unity.exe" -batchmode -quit `
  -projectPath "." `
  -executeMethod ProjetoGrau2.EditorBuild.AndroidBuildAutomation.BuildDevApk `
  -logFile "build_android.log"
```

O arquivo gerado será gravado em `Builds/Android/ReryCity_Araguari_Pilot_v0.1.apk`.

### Opção 3: GitHub Actions (Manual)
O workflow `.github/workflows/android-dev.yml` está configurado para execução manual sob demanda (`workflow_dispatch`). Basta acionar no painel de Actions do repositório, configurando os secrets de licenciamento (`UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`).
