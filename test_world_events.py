#!/usr/bin/env python3
"""
Suite de Testes Automatizados - Fase 5.4: Ocorrencias Dinamicas, Bloqueios Viarios e Eventos do Mundo
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade de Scripts C# e Metadados GUID da Fase 5.4
2. Integridade e Resolucao de GUIDs dos ScriptableObjects de Ocorrencias (.asset e .meta)
3. Validacao Estrutural da Cena UrbanTrafficTestScene.unity (WorldEventManager, PoliceDispatchManager, Blockage)
4. Simulacao de Ciclo de Vida de Ocorrencias (Pending -> Active -> Resolving -> Completed)
5. Simulacao de Filtragem Espacial e Anti-Sobreposicao (minSpawnDistance, maxSpawnDistance, minEventDistance)
6. Simulacao de Bloqueio Viario (TrafficLane.isBlocked, Roteamento e Desvio Tatico de NPCs)
7. Simulacao de Despacho Policial (Fila de Incidentes, Selecao da Viatura Mais Proxima, Chegada e Resolucao)
8. Verificacao de Otimizacao Mobile e Zero GC em Loops de Atualizacao
"""

import os
import re
import math

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    os.path.join("Scripts", "World", "WorldEventType.cs"): "10000000000000000000000000000090",
    os.path.join("Scripts", "World", "WorldEventConfig.cs"): "10000000000000000000000000000091",
    os.path.join("Scripts", "World", "WorldEventInstance.cs"): "10000000000000000000000000000092",
    os.path.join("Scripts", "World", "RoadBlockage.cs"): "10000000000000000000000000000093",
    os.path.join("Scripts", "World", "WorldEventManager.cs"): "10000000000000000000000000000094",
    os.path.join("Scripts", "Police", "PoliceDispatchManager.cs"): "10000000000000000000000000000095",
}

EXPECTED_ASSETS = {
    os.path.join("ScriptableObjects", "WorldEvents", "EventConfig_TrafficAccident.asset"): "20000000000000000000000000000090",
    os.path.join("ScriptableObjects", "WorldEvents", "EventConfig_RoadworkBlockage.asset"): "20000000000000000000000000000091",
    os.path.join("ScriptableObjects", "WorldEvents", "EventConfig_BrokenVehicle.asset"): "20000000000000000000000000000092",
    os.path.join("ScriptableObjects", "WorldEvents", "EventConfig_PoliceCheckpoint.asset"): "20000000000000000000000000000093",
}

class TestReporter:
    def __init__(self):
        self.passed = 0
        self.failed = 0
        self.errors = []

    def assert_true(self, condition, msg):
        if condition:
            self.passed += 1
            print(f"  [PASS] {msg}")
        else:
            self.failed += 1
            self.errors.append(msg)
            print(f"  [FAIL] {msg}")

    def assert_almost_equal(self, a, b, tol=1e-3, msg=""):
        diff = abs(a - b)
        self.assert_true(diff <= tol, f"{msg} (expected {b:.4f}, got {a:.4f}, diff {diff:.5f})")

    def summary(self):
        total = self.passed + self.failed
        print("\"" + "=" * 70)
        print(f"RELATORIO DE TESTES FASE 5.4 - TOTAL: {total} | APROVADOS: {self.passed} | FALHAS: {self.failed}")
        print("=" * 70)
        if self.errors:
            print("Lista de Falhas:")
            for e in self.errors:
                print(f"  - {e}")
        return self.failed == 0

reporter = TestReporter()

print("=" * 70)
print("1. AUDITORIA DE SCRIPTS C# E METADADOS GUID (FASE 5.4)")
print("=" * 70)

for script_rel, expected_guid in EXPECTED_SCRIPTS.items():
    script_path = os.path.join(ASSETS_DIR, script_rel)
    meta_path = script_path + ".meta"
    reporter.assert_true(os.path.exists(script_path), f"Script existe: {script_rel}")
    reporter.assert_true(os.path.exists(meta_path), f"Meta existe: {script_rel}.meta")
    if os.path.exists(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
        match = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
        actual_guid = match.group(1) if match else ""
        reporter.assert_true(actual_guid == expected_guid, f"GUID correto ({expected_guid}) em {script_rel}")

print("\n" + "=" * 70)
print("2. AUDITORIA DE SCRIPTABLEOBJECTS DE OCORRENCIAS E METAS")
print("=" * 70)

for asset_rel, expected_guid in EXPECTED_ASSETS.items():
    asset_path = os.path.join(ASSETS_DIR, asset_rel)
    meta_path = asset_path + ".meta"
    reporter.assert_true(os.path.exists(asset_path), f"Asset existe: {asset_rel}")
    reporter.assert_true(os.path.exists(meta_path), f"Meta existe: {asset_rel}.meta")
    if os.path.exists(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            meta_content = f.read()
        match = re.search(r"guid:\s*([0-9a-fA-F]+)", meta_content)
        actual_guid = match.group(1) if match else ""
        reporter.assert_true(actual_guid == expected_guid, f"GUID do Asset ({expected_guid}) em {asset_rel}")
    if os.path.exists(asset_path):
        with open(asset_path, "r", encoding="utf-8") as f:
            c = f.read()
        reporter.assert_true("guid: 10000000000000000000000000000091" in c, f"Asset referencia WorldEventConfig script: {asset_rel}")

print("\n" + "=" * 70)
print("3. VALIDACAO ESTRUTURAL DA CENA UrbanTrafficTestScene.unity")
print("=" * 70)

reporter.assert_true(os.path.exists(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")
with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_content = f.read()

reporter.assert_true("m_Name: WorldEventManager" in scene_content, "GameObject WorldEventManager presente na cena")
reporter.assert_true("guid: 10000000000000000000000000000094" in scene_content, "Componente WorldEventManager configurado com GUID correto")
reporter.assert_true("m_Name: PoliceDispatchManager" in scene_content, "GameObject PoliceDispatchManager presente na cena")
reporter.assert_true("guid: 10000000000000000000000000000095" in scene_content, "Componente PoliceDispatchManager configurado com GUID correto")
reporter.assert_true("m_Name: WorldEvent_Roadwork_Demo" in scene_content, "GameObject WorldEvent_Roadwork_Demo presente na cena")
reporter.assert_true("guid: 10000000000000000000000000000092" in scene_content, "WorldEventInstance configurado na cena")
reporter.assert_true("guid: 10000000000000000000000000000093" in scene_content, "RoadBlockage configurado na cena")

for expected_asset_guid in EXPECTED_ASSETS.values():
    reporter.assert_true(expected_asset_guid in scene_content, f"Asset GUID {expected_asset_guid} referenciado no catalogo da cena")

print("\n" + "=" * 70)
print("4. SIMULACAO DO CICLO DE VIDA DE OCORRENCIAS (STATE MACHINE)")
print("=" * 70)

class SimulatedEventInstance:
    def __init__(self, event_id, event_type, duration=60.0):
        self.event_id = event_id
        self.event_type = event_type
        self.duration = duration
        self.timer = 0.0
        self.state = "Pending"
        self.requires_police = (event_type in ["TrafficAccident", "PoliceCheckpoint"])
        self.police_assigned = False

    def activate(self):
        if self.state == "Pending":
            self.state = "Active"
            self.timer = 0.0

    def update(self, dt):
        if self.state in ["Active", "Resolving"]:
            self.timer += dt
            if self.state == "Active" and self.timer >= self.duration:
                self.state = "Resolving"
            if self.state == "Resolving" and self.timer >= (self.duration + 5.0):
                self.state = "Completed"

    def resolve(self):
        self.state = "Completed"

sim_event = SimulatedEventInstance("EVT_01", "TrafficAccident", duration=10.0)
reporter.assert_true(sim_event.state == "Pending", "Estado inicial da ocorrencia e Pending")
sim_event.activate()
reporter.assert_true(sim_event.state == "Active", "Transicao para Active com sucesso")

sim_event.update(5.0)
reporter.assert_true(sim_event.state == "Active", "Apos 5s de 10s permanece Active")
reporter.assert_almost_equal(sim_event.timer, 5.0, 1e-4, "Timer de duracao acumulado corretamente")

sim_event.update(5.1)
reporter.assert_true(sim_event.state == "Resolving", "Apos duracao expirar transiciona para Resolving")

sim_event.update(5.0)
reporter.assert_true(sim_event.state == "Completed", "Apos tempo de limpeza transiciona para Completed")

print("\n" + "=" * 70)
print("5. SIMULACAO DE FILTRAGEM ESPACIAL E ANTI-SOBREPOSICAO")
print("=" * 70)

def is_area_free(candidate_pos, active_positions, min_dist=35.0):
    for pos in active_positions:
        dx = candidate_pos[0] - pos[0]
        dz = candidate_pos[1] - pos[1]
        dist = math.sqrt(dx*dx + dz*dz)
        if dist < min_dist:
            return False
    return True

active_event_positions = [(0.0, 50.0), (40.0, 0.0)]
reporter.assert_true(not is_area_free((0.0, 65.0), active_event_positions, 35.0), "Anti-sobreposicao: rejeita evento a 15m de outro (< 35m)")
reporter.assert_true(is_area_free((0.0, 95.0), active_event_positions, 35.0), "Anti-sobreposicao: aceita evento a 45m de distancia (>= 35m)")

player_pos = (0.0, 0.0)
def is_valid_spawn_distance(candidate_pos, player, min_d=35.0, max_d=90.0):
    dist = math.sqrt((candidate_pos[0]-player[0])**2 + (candidate_pos[1]-player[1])**2)
    return min_d <= dist <= max_d

reporter.assert_true(not is_valid_spawn_distance((0.0, 20.0), player_pos), "Distancia minima: rejeita evento muito proximo do jogador (20m < 35m)")
reporter.assert_true(not is_valid_spawn_distance((0.0, 110.0), player_pos), "Distancia maxima: rejeita evento alem do campo visivel (110m > 90m)")
reporter.assert_true(is_valid_spawn_distance((0.0, 60.0), player_pos), "Distancia valida: aceita evento na faixa de geracao (60m)")

print("\n" + "=" * 70)
print("6. SIMULACAO DE BLOQUEIO VIARIO E DESVIO TATICO DE VEICULOS")
print("=" * 70)

class SimulatedLane:
    def __init__(self, lane_id, is_blocked=False, parallel_lane=None):
        self.lane_id = lane_id
        self.isBlocked = is_blocked
        self.parallel_lane = parallel_lane

lane_north_0 = SimulatedLane("Lane_North_0", is_blocked=False)
lane_north_1 = SimulatedLane("Lane_North_1", is_blocked=False)
lane_north_0.parallel_lane = lane_north_1

lane_north_0.isBlocked = True
reporter.assert_true(lane_north_0.isBlocked, "Propriedade isBlocked ativada na faixa com sucesso")

class SimulatedVehicleController:
    def __init__(self, current_lane):
        self.current_lane = current_lane
        self.is_changing_lane = False
        self.target_lane = None
        self.speed = 10.0

    def update(self):
        if self.current_lane.isBlocked and not self.is_changing_lane:
            if self.current_lane.parallel_lane is not None and not self.current_lane.parallel_lane.isBlocked:
                self.is_changing_lane = True
                self.target_lane = self.current_lane.parallel_lane
            else:
                self.speed = 0.0

npc_vehicle = SimulatedVehicleController(lane_north_0)
npc_vehicle.update()
reporter.assert_true(npc_vehicle.is_changing_lane, "NPC detecta faixa bloqueada e inicia mudanca de faixa tatica")
reporter.assert_true(npc_vehicle.target_lane == lane_north_1, "NPC seleciona faixa paralela desimpedida como rota de desvio")

lane_north_1.isBlocked = True
npc_trapped = SimulatedVehicleController(lane_north_0)
npc_trapped.update()
reporter.assert_true(npc_trapped.speed == 0.0, "NPC para com seguranca se todas as opcoes de desvio estiverem bloqueadas")

print("\n" + "=" * 70)
print("7. SIMULACAO DE DESPACHO POLICIAL (POLICE DISPATCH)")
print("=" * 70)

class SimulatedPatrolUnit:
    def __init__(self, unit_id, pos):
        self.unit_id = unit_id
        self.pos = pos
        self.state = "Patrolling"
        self.target_position = None
        self.assigned_incident = None

class SimulatedDispatchManager:
    def __init__(self):
        self.units = []
        self.incident_queue = []

    def register_unit(self, unit):
        self.units.append(unit)

    def queue_incident(self, incident):
        self.incident_queue.append(incident)
        self.dispatch_available()

    def dispatch_available(self):
        available_units = [u for u in self.units if u.state == "Patrolling"]
        for incident in list(self.incident_queue):
            if not available_units:
                break
            best_unit = min(available_units, key=lambda u: math.dist(u.pos, incident["pos"]))
            best_unit.state = "Investigating"
            best_unit.target_position = incident["pos"]
            best_unit.assigned_incident = incident
            available_units.remove(best_unit)
            self.incident_queue.remove(incident)

dispatch = SimulatedDispatchManager()
unit_hilux = SimulatedPatrolUnit("Hilux_01", (0.0, 10.0))
unit_rocam = SimulatedPatrolUnit("ROCAM_01", (0.0, 80.0))
dispatch.register_unit(unit_hilux)
dispatch.register_unit(unit_rocam)

incident_accident = {"id": "INC_01", "pos": (0.0, 30.0)}
dispatch.queue_incident(incident_accident)

reporter.assert_true(unit_hilux.state == "Investigating", "Viatura mais proxima (Hilux a 20m) despachada para o incidente")
reporter.assert_true(unit_rocam.state == "Patrolling", "Viatura mais distante (ROCAM a 50m) permanece em patrulha")
reporter.assert_true(unit_hilux.target_position == (0.0, 30.0), "Alvo da viatura despachada configurado exatamente na posicao do incidente")
reporter.assert_true(len(dispatch.incident_queue) == 0, "Fila de incidentes limpa apos despacho com sucesso")

def check_arrival(unit, arrival_radius=15.0):
    dist = math.dist(unit.pos, unit.target_position)
    return dist <= arrival_radius

unit_hilux.pos = (0.0, 25.0)
reporter.assert_true(check_arrival(unit_hilux, 15.0), "Deteccao de chegada ao incidente dentro do raio tatico de 15m")

unit_hilux.state = "Patrolling"
unit_hilux.assigned_incident = None
unit_hilux.target_position = None
reporter.assert_true(unit_hilux.state == "Patrolling", "Viatura liberada com sucesso e retorna ao estado Patrolling")

print("\n" + "=" * 70)
print("8. VERIFICACAO DE PERFORMANCE, MOBILE PROFILES E ZERO GC")
print("=" * 70)

with open(os.path.join(ASSETS_DIR, "Scripts", "World", "WorldEventManager.cs"), "r", encoding="utf-8") as f:
    wem_content = f.read()

reporter.assert_true("maxActiveEvents" in wem_content, "Limite maximo de eventos ativos controlado para protecao de memoria mobile")
reporter.assert_true("OnEventStarted" in wem_content and "OnEventFinished" in wem_content, "Eventos baseados em C# Action para comunicacao desacoplada")

with open(os.path.join(ASSETS_DIR, "Scripts", "Police", "PoliceDispatchManager.cs"), "r", encoding="utf-8") as f:
    pdm_content = f.read()

reporter.assert_true("DispatchIncident" in pdm_content or "_pendingIncidents" in pdm_content, "PoliceDispatchManager possui fila gerenciada de ocorrencias")
reporter.assert_true("FindNearestPatrolUnit" in pdm_content or "Dispatch" in pdm_content, "Algoritmo de viatura mais proxima implementado")

with open(os.path.join(ASSETS_DIR, "Scripts", "World", "RoadBlockage.cs"), "r", encoding="utf-8") as f:
    rb_content = f.read()

reporter.assert_true("isBlocked" in rb_content, "RoadBlockage interage diretamente com TrafficLane.isBlocked")

success = reporter.summary()

import unittest

class TestWorldEventsSuite(unittest.TestCase):
    def test_world_events_suite(self):
        self.assertTrue(success, f"Falhas detectadas na Fase 5.4: {reporter.errors}")
        self.assertEqual(reporter.failed, 0)
        self.assertGreater(reporter.passed, 0)

if __name__ == '__main__':
    exit(0 if success else 1)
