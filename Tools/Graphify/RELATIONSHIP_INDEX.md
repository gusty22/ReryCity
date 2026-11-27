---
id: RELATIONSHIP_INDEX
tipo: indice_relacional
projeto: Projeto Grau 2
---

# ÍNDICE RELACIONAL DE CONHECIMENTO (GRAPHIFY MAP)
> **Mapa Topológico de Conexões entre Sistemas, Classes, Decisões e Testes.**

**Métricas do Grafo**: 158 nós | 368 arestas de dependência/referência.

## 1. Conexões dos Subsistemas de Jogo (Forward & Backward Dependencies)

### `[[SYS_AI_DRIVER]]` — SYS_AI_DRIVER
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_AI_SENSORS]]` — SYS_AI_SENSORS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_AI_VEHICLES]]` — SYS_AI_VEHICLES
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_ROAD_BLOCKAGE]]`, `[[SYS_WORLD_EVENTS]]`, `[[SYS_WORLD_POPULATION]]`

### `[[SYS_ARAGUARI_GEOGRAPHY]]` — SYS_ARAGUARI_GEOGRAPHY
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_ATHLON]]` — SYS_ATHLON
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_2`
* **Depende de (Outbound)**: `[[SYS_ENGINE]]`, `[[SYS_AUDIO]]`, `[[AthlonECUData]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[ANALISE_FISICA_MOTOS]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_ATHLON_MOBILE]]`, `[[SYS_AUDIO]]`, `[[SYS_AUDIO_INTEGRATION]]`, `[[SYS_ENGINE]]`, `[[SYS_SAVE_SYSTEM]]`, `[[SYS_MOTORCYCLE_HUD]]`

### `[[SYS_ATHLON_MOBILE]]` — SYS_ATHLON_MOBILE
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_3`
* **Depende de (Outbound)**: `[[SYS_ATHLON]]`, `[[SYS_INPUT]]`, `[[FORMULAS_FISICA_MOTOS]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_MOBILE_CONTROLS]]`

### `[[SYS_AUDIO]]` — SYS_AUDIO
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_2`
* **Depende de (Outbound)**: `[[SYS_ENGINE]]`, `[[SYS_TRANSMISSION]]`, `[[SYS_ATHLON]]`, `[[MotorAudioProfile]]`, `[[ADR_003]]`, `[[ADR_002]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_ATHLON]]`, `[[SYS_AUDIO_INTEGRATION]]`

### `[[SYS_AUDIO_INTEGRATION]]` — SYS_AUDIO_INTEGRATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_AUDIO]]`, `[[SYS_ATHLON]]`, `[[SYS_ENGINE]]`, `[[SYS_POLICE_PURSUIT]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_BRAKES]]` — SYS_BRAKES
* **Tipo / Status**: `sistema` | `ESPECIFICADO`
* **Depende de (Outbound)**: `[[SYS-01]]`, `[[SYS-04]]`, `[[SYS-07]]`, `[[Rigidbody]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[MAPEAMENTO_FISICA_MOTOCICLETAS]]`, `[[SYS_SUSPENSION]]`, `[[SYS_INPUT]]`, `[[SYS_WHEELIE]]`, `[[SYS_MOTORCYCLE]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_1_PHYSICS_AUDIT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_SUSPENSION]]`, `[[SYS_MOBILE_CONTROLS]]`, `[[SYS_WHEELIE]]`

### `[[SYS_CAMERA]]` — SYS_CAMERA
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_3`
* **Depende de (Outbound)**: `[[SYS_MOTORCYCLE]]`, `[[SYS_WHEELIE]]`, `[[Rigidbody]]`, `[[ADR_002]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_5_PRE_AUDIT_REPORT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_MOTORCYCLE_HUD]]`

### `[[SYS_CITY_MATERIALS]]` — SYS_CITY_MATERIALS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_CUSTOMIZATION]]` — SYS_CUSTOMIZATION
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_3`
* **Depende de (Outbound)**: `[[SYS_WORKSHOP]]`, `[[SYS_ENGINE]]`, `[[SYS_TRANSMISSION]]`, `[[SYS_SUSPENSION]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[ADR_003]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_SUSPENSION]]`, `[[SYS_WORKSHOP]]`

### `[[SYS_ECONOMY]]` — SYS_ECONOMY
* **Tipo / Status**: `external_or_unresolved` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_SAVE_SYSTEM]]`

### `[[SYS_ENERGY_MANAGEMENT]]` — SYS_ENERGY_MANAGEMENT
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_ENGINE]]` — SYS_ENGINE
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_2`
* **Depende de (Outbound)**: `[[SYS_TRANSMISSION]]`, `[[SYS_ATHLON]]`, `[[EngineData]]`, `[[Rigidbody]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[ADR_003]]`, `[[ADR_002]]`, `[[MAPEAMENTO_FISICA_MOTOCICLETAS]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_5_PRE_AUDIT_REPORT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_ATHLON]]`, `[[SYS_AUDIO]]`, `[[SYS_AUDIO_INTEGRATION]]`, `[[SYS_CUSTOMIZATION]]`, `[[SYS_TRANSMISSION]]`, `[[SYS_MOTORCYCLE_HUD]]`, `[[SYS_WORKSHOP]]`

### `[[SYS_FACADE_DETAILS]]` — SYS_FACADE_DETAILS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_GAME_IDENTITY]]` — SYS_GAME_IDENTITY
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_GAME_LOADING]]` — SYS_GAME_LOADING
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_GARAGE]]` — SYS_GARAGE
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[RELATIONSHIP_INDEX]]`

### `[[SYS_GEOGRAPHIC_COORDINATES]]` — SYS_GEOGRAPHIC_COORDINATES
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_IBGE_FAITHFUL_RECONSTRUCTION]]` — SYS_IBGE_FAITHFUL_RECONSTRUCTION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_INPUT]]` — SYS_INPUT
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_ATHLON_MOBILE]]`, `[[SYS_BRAKES]]`, `[[SYS_MOBILE_CONTROLS]]`

### `[[SYS_INTERACTIVE_ESTABLISHMENTS]]` — SYS_INTERACTIVE_ESTABLISHMENTS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_INTERIOR_STREAMING]]` — SYS_INTERIOR_STREAMING
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_MAP_STREAMING_INTEGRATION]]` — SYS_MAP_STREAMING_INTEGRATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_MISSIONS]]` — SYS_MISSIONS
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_MOBILE_CONTROLS]]` — SYS_MOBILE_CONTROLS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WHEELIE]]`, `[[SYS_INPUT]]`, `[[SYS_ATHLON_MOBILE]]`, `[[SYS_BRAKES]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_MOTORCYCLE]]` — SYS_MOTORCYCLE
* **Tipo / Status**: `external_or_unresolved` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_BRAKES]]`, `[[SYS_CAMERA]]`, `[[SYS_RIDER]]`

### `[[SYS_MOTORCYCLE_HUD]]` — SYS_MOTORCYCLE_HUD
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_TRANSMISSION]]`, `[[SYS_ATHLON]]`, `[[SYS_WHEELIE]]`, `[[SYS_ENGINE]]`, `[[SYS_CAMERA]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_NEIGHBORHOOD_IDENTITY]]` — SYS_NEIGHBORHOOD_IDENTITY
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_NPC]]` — SYS_NPC
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[RELATIONSHIP_INDEX]]`

### `[[SYS_OFFLINE_INITIALIZATION]]` — SYS_OFFLINE_INITIALIZATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_PHONE_GPS]]` — SYS_PHONE_GPS
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_PLAYER]]` — SYS_PLAYER
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[RELATIONSHIP_INDEX]]`

### `[[SYS_PLAYER_INTERACTION]]` — SYS_PLAYER_INTERACTION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_POINT_OF_INTEREST]]` — SYS_POINT_OF_INTEREST
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_POLICE]]` — SYS_POLICE
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[RELATIONSHIP_INDEX]]`

### `[[SYS_POLICE_AI]]` — SYS_POLICE_AI
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_PURSUIT_COORDINATION]]`

### `[[SYS_POLICE_DISPATCH]]` — SYS_POLICE_DISPATCH
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_POLICE_AI]]`, `[[SYS_WORLD_EVENTS]]`, `[[SYS_AI_VEHICLES]]`, `[[SYS_POLICE_PURSUIT]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_PURSUIT_COORDINATION]]`, `[[SYS_WORLD_EVENTS]]`

### `[[SYS_POLICE_HUD]]` — SYS_POLICE_HUD
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_POLICE_PURSUIT]]` — SYS_POLICE_PURSUIT
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_AUDIO_INTEGRATION]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_PURSUIT_COORDINATION]]`

### `[[SYS_PROCEDURAL_ENVIRONMENT]]` — SYS_PROCEDURAL_ENVIRONMENT
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WORLD_STREAMING]]`, `[[SYS_WORLD]]`, `[[SYS_TRAFFIC_NAVIGATION]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_WORLD_STREAMING]]`

### `[[SYS_PURSUIT_COORDINATION]]` — SYS_PURSUIT_COORDINATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_TRAFFIC_NAVIGATION]]`, `[[SYS_POLICE_AI]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_POLICE_PURSUIT]]`, `[[SYS_WANTED_LEVEL]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_REAL_ROAD_NETWORK]]` — SYS_REAL_ROAD_NETWORK
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_RIDER]]` — SYS_RIDER
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_4`
* **Depende de (Outbound)**: `[[SYS_MOTORCYCLE]]`, `[[SYS_WHEELIE]]`, `[[RiderData]]`, `[[ADR_003]]`, `[[ADR_002]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_5_PRE_AUDIT_REPORT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_RIDER_ANIMATIONS]]`, `[[SYS_RIDER_EQUIPMENT]]`, `[[SYS_RIDER_IK]]`, `[[SYS_RIDER_RAGDOLL]]`

### `[[SYS_RIDER_ANIMATIONS]]` — SYS_RIDER_ANIMATIONS
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_4`
* **Depende de (Outbound)**: `[[SYS_RIDER]]`, `[[SYS_WHEELIE]]`, `[[RiderData]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_RIDER_EQUIPMENT]]` — SYS_RIDER_EQUIPMENT
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_4`
* **Depende de (Outbound)**: `[[SYS_RIDER]]`, `[[SYS_WORKSHOP]]`, `[[RiderEquipmentData]]`, `[[ADR_003]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_RIDER_IK]]` — SYS_RIDER_IK
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_4`
* **Depende de (Outbound)**: `[[SYS_RIDER]]`, `[[Animator]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_RIDER_RAGDOLL]]` — SYS_RIDER_RAGDOLL
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_4`
* **Depende de (Outbound)**: `[[SYS_RIDER]]`, `[[Rigidbody]]`, `[[CharacterJoint]]`, `[[SYS_WHEELIE]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_ROAD_BLOCKAGE]]` — SYS_ROAD_BLOCKAGE
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WORLD_EVENTS]]`, `[[SYS_TRAFFIC_NAVIGATION]]`, `[[SYS_AI_VEHICLES]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_WORLD_EVENTS]]`

### `[[SYS_SAVE]]` — SYS_SAVE
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[RELATIONSHIP_INDEX]]`

### `[[SYS_SAVE_SYSTEM]]` — SYS_SAVE_SYSTEM
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_3`
* **Depende de (Outbound)**: `[[SYS_WORKSHOP]]`, `[[SYS_ATHLON]]`, `[[SYS_ECONOMY]]`, `[[ADR_003]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_SIMULATED_REWARDS]]` — SYS_SIMULATED_REWARDS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_STREET_INFRASTRUCTURE]]` — SYS_STREET_INFRASTRUCTURE
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_SUSPENSION]]` — SYS_SUSPENSION
* **Tipo / Status**: `sistema` | `ESPECIFICADO`
* **Depende de (Outbound)**: `[[SYS-07]]`, `[[Rigidbody]]`, `[[WheelCollider]]`, `[[SYS_CUSTOMIZATION]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[MAPEAMENTO_FISICA_MOTOCICLETAS]]`, `[[SYS_WHEELIE]]`, `[[SYS_BRAKES]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_1_PHYSICS_AUDIT]]`, `[[AGENT_START_HERE]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_BRAKES]]`, `[[SYS_CUSTOMIZATION]]`

### `[[SYS_TRAFFIC_NAVIGATION]]` — SYS_TRAFFIC_NAVIGATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_PURSUIT_COORDINATION]]`, `[[SYS_ROAD_BLOCKAGE]]`, `[[SYS_PROCEDURAL_ENVIRONMENT]]`, `[[SYS_WORLD_EVENTS]]`, `[[SYS_WORLD_POPULATION]]`

### `[[SYS_TRAFFIC_OPTIMIZATION]]` — SYS_TRAFFIC_OPTIMIZATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_WORLD_OPTIMIZATION]]`

### `[[SYS_TRAFFIC_SPAWNING]]` — SYS_TRAFFIC_SPAWNING
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_TRAFFIC_VIOLATIONS]]` — SYS_TRAFFIC_VIOLATIONS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_TRANSMISSION]]` — SYS_TRANSMISSION
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_2`
* **Depende de (Outbound)**: `[[SYS_ENGINE]]`, `[[TransmissionData]]`, `[[Rigidbody]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[MAPEAMENTO_FISICA_MOTOCICLETAS]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_5_PRE_AUDIT_REPORT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_AUDIO]]`, `[[SYS_CUSTOMIZATION]]`, `[[SYS_ENGINE]]`, `[[SYS_MOTORCYCLE_HUD]]`, `[[SYS_WORKSHOP]]`

### `[[SYS_UI]]` — SYS_UI
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_URBAN_FURNITURE_LIGHTING]]` — SYS_URBAN_FURNITURE_LIGHTING
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_URBAN_LOTS_BUILDINGS]]` — SYS_URBAN_LOTS_BUILDINGS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_WANTED_LEVEL]]` — SYS_WANTED_LEVEL
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_PURSUIT_COORDINATION]]`

### `[[SYS_WHEELIE]]` — SYS_WHEELIE
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_1`
* **Depende de (Outbound)**: `[[SYS-02]]`, `[[SYS-04]]`, `[[SYS-05]]`, `[[SYS-07]]`, `[[Rigidbody]]`, `[[MotorcycleData]]`, `[[IMotorcycleInput]]`, `[[ADR_004]]`, `[[FORMULAS_FISICA_MOTOS]]`, `[[MAPEAMENTO_FISICA_MOTOCICLETAS]]`, `[[ADR_003]]`, `[[ADR_002]]`, `[[SYS_BRAKES]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[FASE_1_PHYSICS_AUDIT]]`, `[[FASE_5_PRE_AUDIT_REPORT]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_BRAKES]]`, `[[SYS_CAMERA]]`, `[[SYS_RIDER]]`, `[[SYS_RIDER_ANIMATIONS]]`, `[[SYS_RIDER_RAGDOLL]]`, `[[SYS_SUSPENSION]]`, `[[SYS_MOBILE_CONTROLS]]`, `[[SYS_MOTORCYCLE_HUD]]`

### `[[SYS_WORKSHOP]]` — SYS_WORKSHOP
* **Tipo / Status**: `sistema` | `IMPLEMENTADO_EM_FASE_3`
* **Depende de (Outbound)**: `[[SYS_CUSTOMIZATION]]`, `[[SYS_ENGINE]]`, `[[SYS_TRANSMISSION]]`, `[[PartItemData]]`, `[[ADR_003]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_CUSTOMIZATION]]`, `[[SYS_RIDER_EQUIPMENT]]`, `[[SYS_SAVE_SYSTEM]]`

### `[[SYS_WORLD]]` — SYS_WORLD
* **Tipo / Status**: `unresolved_link` | `MISSING`
* **Depende de (Outbound)**: Nenhum (Nó Base / Folha)
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_PROCEDURAL_ENVIRONMENT]]`, `[[SYS_WORLD_OPTIMIZATION]]`, `[[SYS_WORLD_POPULATION]]`, `[[SYS_WORLD_STREAMING]]`

### `[[SYS_WORLD_EVENTS]]` — SYS_WORLD_EVENTS
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_TRAFFIC_NAVIGATION]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_AI_VEHICLES]]`, `[[SYS_ROAD_BLOCKAGE]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_POLICE_DISPATCH]]`, `[[SYS_ROAD_BLOCKAGE]]`

### `[[SYS_WORLD_OPTIMIZATION]]` — SYS_WORLD_OPTIMIZATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WORLD_STREAMING]]`, `[[SYS_WORLD]]`, `[[SYS_TRAFFIC_OPTIMIZATION]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_WORLD_STREAMING]]`

### `[[SYS_WORLD_POPULATION]]` — SYS_WORLD_POPULATION
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WORLD]]`, `[[SYS_TRAFFIC_NAVIGATION]]`, `[[SYS_AI_VEHICLES]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`

### `[[SYS_WORLD_STREAMING]]` — SYS_WORLD_STREAMING
* **Tipo / Status**: `documento` | ``
* **Depende de (Outbound)**: `[[SYS_WORLD]]`, `[[SYS_WORLD_OPTIMIZATION]]`, `[[SYS_PROCEDURAL_ENVIRONMENT]]`
* **Impacta Diretamente (Inbound)**: `[[PROJECT_INDEX]]`, `[[RELATIONSHIP_INDEX]]`, `[[SYS_PROCEDURAL_ENVIRONMENT]]`, `[[SYS_WORLD_OPTIMIZATION]]`

## 2. Mapa Rápido de Impacto por Subsistema
| Se você alterar... | Você DEVE verificar os seguintes módulos afetados: |
| :--- | :--- |
| `[[SYS_BRAKES]]` | [[FASE_1_PHYSICS_AUDIT]], [[RELATIONSHIP_INDEX]], [[SYS_SUSPENSION]], [[SYS_MOBILE_CONTROLS]], [[SYS_WHEELIE]] |
| `[[SYS_SUSPENSION]]` | [[FASE_1_PHYSICS_AUDIT]], [[RELATIONSHIP_INDEX]], [[SYS_BRAKES]], [[SYS_CUSTOMIZATION]] |
| `[[SYS_WHEELIE]]` | [[FASE_1_PHYSICS_AUDIT]], [[FASE_5_PRE_AUDIT_REPORT]], [[RELATIONSHIP_INDEX]], [[SYS_BRAKES]], [[SYS_CAMERA]], [[SYS_RIDER]], [[SYS_RIDER_ANIMATIONS]], [[SYS_RIDER_RAGDOLL]], [[SYS_SUSPENSION]], [[SYS_MOBILE_CONTROLS]], [[SYS_MOTORCYCLE_HUD]] |
| `[[SYS_ENGINE]]` | [[FASE_5_PRE_AUDIT_REPORT]], [[RELATIONSHIP_INDEX]], [[SYS_ATHLON]], [[SYS_AUDIO]], [[SYS_AUDIO_INTEGRATION]], [[SYS_CUSTOMIZATION]], [[SYS_TRANSMISSION]], [[SYS_MOTORCYCLE_HUD]], [[SYS_WORKSHOP]] |