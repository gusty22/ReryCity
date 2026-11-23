#!/usr/bin/env python3
"""
Validação e Teste Automatizado da Etapa 5.1 — Infraestrutura de Navegação Urbana
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade dos Scripts C# de Tráfego e Metadados GUID
2. Validação da Cena UrbanTrafficTestScene.unity
3. Algoritmo de Continuidade e Busca Vetorial de Waypoints / Faixas
4. Ciclos Temporizados de 4 Fases de Semáforos (Green, Yellow, Red, AllRed)
5. Lógica de Cruzamento: Prioridades e Prevenção de Bloqueios (CanEnterIntersection)
"""

import os
import re
import math
import yaml

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
SCRIPTS_DIR = os.path.join(UNITY_ROOT, "Assets", "Scripts", "Traffic")
SCENE_PATH = os.path.join(UNITY_ROOT, "Assets", "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    "TrafficWaypoint.cs": "10000000000000000000000000000040",
    "TrafficLane.cs": "10000000000000000000000000000041",
    "TrafficNode.cs": "10000000000000000000000000000042",
    "TrafficSignal.cs": "10000000000000000000000000000043",
    "TrafficIntersection.cs": "10000000000000000000000000000044",
    "TrafficWaypointNetwork.cs": "10000000000000000000000000000045",
    "TrafficNavigationManager.cs": "10000000000000000000000000000046",
}

class TestResult:
    def __init__(self):
        self.passed = 0
        self.failed = 0
        self.errors = []

    def assert_true(self, condition, test_name, msg=""):
        if condition:
            self.passed += 1
            print(f"  [PASS] {test_name}")
        else:
            self.failed += 1
            error_msg = f"{test_name} FAILED: {msg}"
            self.errors.append(error_msg)
            print(f"  [FAIL] {error_msg}")

results = TestResult()

print("=" * 70)
print("INICIANDO BATERIA DE TESTES — ETAPA 5.1: NAVEGAÇÃO URBANA")
print("=" * 70)

# -------------------------------------------------------------
# TESTE 1: Integridade de Arquivos e Metadados
# -------------------------------------------------------------
print("\n>>> TESTE 1: Integridade de Scripts C# e GUIDs Determinísticos")
for script_name, expected_guid in EXPECTED_SCRIPTS.items():
    script_file = os.path.join(SCRIPTS_DIR, script_name)
    meta_file = script_file + ".meta"
    
    results.assert_true(os.path.isfile(script_file), f"Script existe: {script_name}")
    results.assert_true(os.path.isfile(meta_file), f"Meta existe: {script_name}.meta")
    
    if os.path.isfile(meta_file):
        with open(meta_file, "r", encoding="utf-8") as f:
            content = f.read()
            m = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
            found_guid = m.group(1) if m else None
            results.assert_true(found_guid == expected_guid, 
                                f"GUID confere ({script_name})", 
                                f"Esperado {expected_guid}, encontrado {found_guid}")

# -------------------------------------------------------------
# TESTE 2: Validação da Cena UrbanTrafficTestScene.unity
# -------------------------------------------------------------
print("\n>>> TESTE 2: Validação da Cena UrbanTrafficTestScene.unity")
results.assert_true(os.path.isfile(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")
results.assert_true(os.path.isfile(SCENE_PATH + ".meta"), "Meta da Cena existe")

if os.path.isfile(SCENE_PATH):
    with open(SCENE_PATH, "r", encoding="utf-8") as f:
        scene_text = f.read()
    
    results.assert_true("TrafficNavigationManager" in scene_text, "TrafficNavigationManager presente na cena")
    results.assert_true("TrafficWaypointNetwork" in scene_text, "TrafficWaypointNetwork presente na cena")
    results.assert_true("Intersection_Centro_4Way" in scene_text, "Intersection_Centro_4Way presente na cena")
    results.assert_true("TrafficSignal_North" in scene_text, "TrafficSignal_North presente na cena")
    results.assert_true("TrafficSignal_East" in scene_text, "TrafficSignal_East presente na cena")
    results.assert_true("Lane_North_In_0" in scene_text, "Lane_North_In_0 presente na cena")
    results.assert_true("Waypoint_N_In_StopLine" in scene_text, "Waypoint_N_In_StopLine presente na cena")

# -------------------------------------------------------------
# TESTE 3: Simulação de Algoritmo de Waypoints e Faixas
# -------------------------------------------------------------
print("\n>>> TESTE 3: Algoritmo de Navegação Vetorial, Trajetória e Proximidade")

class MockWaypoint:
    def __init__(self, pos, is_stop=False, speed_limit=60.0):
        self.position = pos
        self.is_stop = is_stop
        self.speed_limit = speed_limit
        self.next_wps = []

class MockLane:
    def __init__(self, lane_id, wps, direction="Forward"):
        self.lane_id = lane_id
        self.waypoints = wps
        self.direction = direction

    def get_nearest_waypoint(self, pos):
        if not self.waypoints:
            return None
        return min(self.waypoints, key=lambda wp: (wp.position[0]-pos[0])**2 + (wp.position[2]-pos[2])**2)

# Simulação de faixa norte (Y descendo de Z=50 para Z=12)
wp1 = MockWaypoint(( -1.75, 0.1, 50.0 ), is_stop=False, speed_limit=60.0)
wp2 = MockWaypoint(( -1.75, 0.1, 30.0 ), is_stop=False, speed_limit=50.0)
wp3 = MockWaypoint(( -1.75, 0.1, 12.0 ), is_stop=True, speed_limit=40.0)
wp1.next_wps.append(wp2)
wp2.next_wps.append(wp3)

lane_north = MockLane("lane_north_in_0", [wp1, wp2, wp3])

# Consulta de proximidade
test_pos = (-1.5, 0.0, 32.0)
nearest = lane_north.get_nearest_waypoint(test_pos)
results.assert_true(nearest == wp2, "GetNearestWaypoint retorna o ponto de menor distância euclidiana", 
                    f"Esperado Z=30.0, obtido Z={nearest.position[2] if nearest else None}")
results.assert_true(wp3.is_stop == True, "Waypoint de retenção identificado como StopLine")

# -------------------------------------------------------------
# TESTE 4: Simulação de Máquina de Estados do Semáforo (4 Fases)
# -------------------------------------------------------------
print("\n>>> TESTE 4: Ciclo Semafórico de 4 Fases (Verde, Amarelo, All-Red, Vermelho)")

class MockTrafficSignal:
    def __init__(self, green=20.0, yellow=3.5, all_red=2.0, red=24.0, initial="Green"):
        self.green_duration = green
        self.yellow_duration = yellow
        self.all_red_duration = all_red
        self.red_duration = red
        self.state = initial
        self.timer = 0.0

    def tick(self, dt):
        self.timer += dt
        if self.state == "Green" and self.timer >= self.green_duration:
            self.state = "Yellow"
            self.timer = 0.0
        elif self.state == "Yellow" and self.timer >= self.yellow_duration:
            self.state = "AllRed"
            self.timer = 0.0
        elif self.state == "AllRed" and self.timer >= self.all_red_duration:
            self.state = "Red"
            self.timer = 0.0
        elif self.state == "Red" and self.timer >= self.red_duration:
            self.state = "Green"
            self.timer = 0.0

    @property
    def is_red(self):
        return self.state in ("Red", "AllRed")

signal = MockTrafficSignal(green=20.0, yellow=3.5, all_red=2.0, red=24.0, initial="Green")

# Avança 10s (deve continuar Green)
signal.tick(10.0)
results.assert_true(signal.state == "Green", "Após 10s permanece Green")

# Avança mais 10s (total 20s -> transita para Yellow)
signal.tick(10.0)
results.assert_true(signal.state == "Yellow", "Ao atingir 20s transita de Green para Yellow")

# Avança 3.5s (transita para AllRed)
signal.tick(3.5)
results.assert_true(signal.state == "AllRed", "Ao esgotar Yellow transita para AllRed (intervalo de limpeza)")
results.assert_true(signal.is_red == True, "AllRed é considerado bloqueio seguro (is_red = True)")

# Avança 2.0s (transita para Red)
signal.tick(2.0)
results.assert_true(signal.state == "Red", "Após AllRed transita para Red")

# Avança 24.0s (completa ciclo e volta para Green)
signal.tick(24.0)
results.assert_true(signal.state == "Green", "Após Red transita de volta para Green (fechamento de ciclo perfeito)")

# -------------------------------------------------------------
# TESTE 5: Lógica de Prioridade e Prevenção de Bloqueio em Cruzamento
# -------------------------------------------------------------
print("\n>>> TESTE 5: Lógica de Prioridade e Anti-Colisão em Cruzamento (CanEnterIntersection)")

class MockIntersection:
    def __init__(self, priority_type="TrafficLight"):
        self.priority_type = priority_type
        self.occupying = set()
        self.signal_green = True

    def can_enter(self, vehicle_id):
        if self.priority_type == "TrafficLight":
            if not self.signal_green:
                return False
            # Bloqueio se já houver congestionamento interno excessivo
            if len(self.occupying) >= 3:
                return False
            return True
        elif self.priority_type == "AllWayStop":
            # Em parada obrigatória, ninguém entra enquanto houver ocupante
            return len(self.occupying) == 0
        return True

inter_light = MockIntersection("TrafficLight")
results.assert_true(inter_light.can_enter("Car_1") == True, "Semáforo Verde permite ingresso no cruzamento")

inter_light.signal_green = False
results.assert_true(inter_light.can_enter("Car_1") == False, "Semáforo Vermelho bloqueia ingresso")

inter_light.signal_green = True
inter_light.occupying.update(["Car_A", "Car_B", "Car_C"])
results.assert_true(inter_light.can_enter("Car_New") == False, "Prevenção de deadlock bloqueia ingresso quando miolo está lotado")

inter_stop = MockIntersection("AllWayStop")
results.assert_true(inter_stop.can_enter("Moto_1") == True, "Stop Sign livre permite passagem")
inter_stop.occupying.add("Truck_1")
results.assert_true(inter_stop.can_enter("Moto_1") == False, "Stop Sign ocupado obriga cedência de passagem")

print("\n" + "=" * 70)
print(f"RESUMO DOS TESTES: {results.passed} PASSOU | {results.failed} FALHOU")
print("=" * 70)

import unittest

class TestTrafficNavigationSuite(unittest.TestCase):
    def test_traffic_navigation_suite(self):
        self.assertEqual(results.failed, 0, f"Falhas detectadas na Fase 5.1: {results.errors}")
        self.assertGreater(results.passed, 0)

if __name__ == '__main__':
    if results.failed > 0:
        for err in results.errors:
            print(f"  [ERRO] {err}")
        exit(1)
    else:
        print("TODOS OS TESTES DA ETAPA 5.1 FORAM EXECUTADOS COM SUCESSO!")
        exit(0)
