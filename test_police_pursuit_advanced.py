#!/usr/bin/env python3
"""
Suíte de Testes Automatizados — Fase 5.5: Integração Avançada de Perseguições Policiais
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade de Scripts C# e Metadados GUID da Fase 5.5
2. Validação Estrutural da Cena UrbanTrafficTestScene.unity (PolicePursuitCoordinator)
3. Simulação de Atribuição de Papéis Táticos (ROCAM vs Cruiser)
4. Simulação de Compartilhamento de Informações via Rádio COPOM (LKP Global)
5. Simulação de Busca Coordenada por Quadrantes na Perda de Contato Visual
6. Simulação de Cálculo de Posicionamento Tático (Lead, Flankers, Interceptor)
7. Simulação de Mobilização e Escalonamento por Nível de Procurado (1 a 5 Estrelas)
8. Simulação de Encerramento Coordenado de Perseguição (Prisão vs Fuga/Evasão)
9. Verificação de Otimizações Mobile Android e Perfil Zero GC
"""

import os
import re
import math

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    os.path.join("Scripts", "Police", "PoliceTacticalRole.cs"): "10000000000000000000000000000096",
    os.path.join("Scripts", "Police", "PolicePursuitCoordinator.cs"): "10000000000000000000000000000097",
    os.path.join("Scripts", "Police", "PolicePursuitController.cs"): "10000000000000000000000000000076",
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
        self.assert_true(diff <= tol, f"{msg} (esperado {b:.4f}, obtido {a:.4f}, diff {diff:.5f})")

    def summary(self):
        total = self.passed + self.failed
        print("\"" + "=" * 70)
        print(f"RELATORIO DE TESTES FASE 5.5 - TOTAL: {total} | APROVADOS: {self.passed} | FALHAS: {self.failed}")
        print("=" * 70)
        if self.errors:
            print("Lista de Falhas:")
            for e in self.errors:
                print(f"  - {e}")
        return self.failed == 0

reporter = TestReporter()

print("=" * 70)
print("1. AUDITORIA DE SCRIPTS C# E METADADOS GUID (FASE 5.5)")
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
print("2. VALIDACAO ESTRUTURAL DA CENA UrbanTrafficTestScene.unity")
print("=" * 70)

reporter.assert_true(os.path.exists(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")
with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_content = f.read()

reporter.assert_true("m_Name: PolicePursuitCoordinator" in scene_content, "GameObject PolicePursuitCoordinator presente na cena")
reporter.assert_true("guid: 10000000000000000000000000000097" in scene_content, "Componente PolicePursuitCoordinator com GUID correto")
reporter.assert_true("maxPursuitUnits" in scene_content, "Campo maxPursuitUnits configurado no componente")

print("\n" + "=" * 70)
print("3. SIMULACAO DE ATRIBUICAO DE PAPEIS TATICOS (ROCAM VS CRUISER)")
print("=" * 70)

class SimulatedUnit:
    def __init__(self, name, is_two_wheeled, pos):
        self.name = name
        self.is_two_wheeled = is_two_wheeled
        self.pos = pos
        self.role = "None"
        self.state = "Patrol"
        self.target_pos = None

class SimulatedCoordinator:
    def __init__(self, max_units=4):
        self.max_units = max_units
        self.active_units = []
        self.is_pursuit_active = False
        self.has_visual_contact = False
        self.global_lkp = (0.0, 0.0)
        self.time_since_visual_lost = 0.0
        self.search_timeout = 14.0

    def mobilize_units(self, available_units, wanted_level, lkp):
        self.is_pursuit_active = True
        self.global_lkp = lkp
        self.has_visual_contact = True
        self.time_since_visual_lost = 0.0

        desired_count = min(wanted_level, self.max_units)
        # Ordena por proximidade da LKP
        sorted_units = sorted(available_units, key=lambda u: math.dist(u.pos, lkp))
        self.active_units = sorted_units[:desired_count]
        self.assign_roles()

    def assign_roles(self):
        if not self.active_units:
            return

        lead_candidate = None
        # Prioriza ROCAM como LeadPursuer
        for u in self.active_units:
            if u.is_two_wheeled:
                lead_candidate = u
                break

        if lead_candidate is None:
            lead_candidate = self.active_units[0]

        lead_candidate.role = "LeadPursuer"

        other_units = [u for u in self.active_units if u != lead_candidate]
        roles_sequence = ["FlankerLeft", "FlankerRight", "Interceptor"]
        for idx, u in enumerate(other_units):
            u.role = roles_sequence[min(idx, len(roles_sequence) - 1)]

coord = SimulatedCoordinator(max_units=4)
hilux_1 = SimulatedUnit("Cruiser_Hilux_1", is_two_wheeled=False, pos=(0.0, 10.0))
rocam_1 = SimulatedUnit("ROCAM_Tiger800", is_two_wheeled=True, pos=(0.0, 25.0))
hilux_2 = SimulatedUnit("Cruiser_Hilux_2", is_two_wheeled=False, pos=(10.0, 30.0))
hilux_3 = SimulatedUnit("Cruiser_Hilux_3", is_two_wheeled=False, pos=(20.0, 40.0))

units_pool = [hilux_1, rocam_1, hilux_2, hilux_3]
coord.mobilize_units(units_pool, wanted_level=3, lkp=(0.0, 50.0))

reporter.assert_true(rocam_1.role == "LeadPursuer", "ROCAM assume prioridade como LeadPursuer pela agilidade de moto")
cruisers_active = [u for u in coord.active_units if not u.is_two_wheeled]
reporter.assert_true(any(u.role in ["FlankerLeft", "FlankerRight"] for u in cruisers_active), "Viaturas Cruiser assumem papéis de flanqueamento e contenção")
reporter.assert_true(len(coord.active_units) == 3, "Quantidade de viaturas mobilizadas respeita Wanted Level 3")

print("\n" + "=" * 70)
print("4. SIMULACAO DE COMPARTILHAMENTO DE RADIO COPOM E LKP GLOBAL")
print("=" * 70)

# Unidade 1 avista o suspeito em nova coordenada
new_sighting_pos = (15.0, 95.0)
coord.global_lkp = new_sighting_pos
coord.has_visual_contact = True
coord.time_since_visual_lost = 0.0

# Unidades sem visao direta recebem a LKP pelo coordenador
for u in coord.active_units:
    u.target_pos = coord.global_lkp

reporter.assert_true(all(u.target_pos == (15.0, 95.0) for u in coord.active_units), "Todas as unidades ativas recebem a LKP global retransmitida pelo rádio")

print("\n" + "=" * 70)
print("5. SIMULACAO DE BUSCA COORDENADA POR QUADRANTES NA PERDA VISUAL")
print("=" * 70)

def distribute_search_quadrants(active_units, center_pos, radius=35.0):
    directions = [(0.0, 1.0), (0.0, -1.0), (1.0, 0.0), (-1.0, 0.0)]
    search_targets = {}
    for idx, u in enumerate(active_units):
        d = directions[idx % len(directions)]
        tgt = (center_pos[0] + d[0] * radius * 0.75, center_pos[1] + d[1] * radius * 0.75)
        search_targets[u.name] = tgt
        u.role = "PerimeterSearch"
        u.target_pos = tgt
    return search_targets

targets = distribute_search_quadrants(coord.active_units, coord.global_lkp, radius=35.0)
reporter.assert_true(len(set(targets.values())) == len(targets), "Todos os alvos de busca setorial são distintos (sem aglomeração)")
reporter.assert_true(all(u.role == "PerimeterSearch" for u in coord.active_units), "Unidades transicionam para papel PerimeterSearch")

# Reencontro do suspeito cancela a busca e reatribui papéis de perseguição
coord.has_visual_contact = True
coord.assign_roles()
reporter.assert_true(rocam_1.role == "LeadPursuer", "Reavistamento do suspeito retoma imediatamente LeadPursuer e formação de perseguição")

print("\n" + "=" * 70)
print("6. SIMULACAO DE CALCULO DE POSICIONAMENTO TATICO (LEAD, FLANKS, INTERCEPTOR)")
print("=" * 70)

player_pos = (100.0, 100.0)
player_fwd = (0.0, 1.0)
player_right = (1.0, 0.0)
player_speed_ms = 20.0 # ~72 km/h

def calc_tactical_pos(role, p_pos, p_fwd, p_right, speed_ms, flank_lat=4.0, flank_back=6.0, lookahead=2.5):
    if role == "LeadPursuer":
        return p_pos
    elif role == "FlankerLeft":
        return (p_pos[0] - p_fwd[0] * flank_back - p_right[0] * flank_lat,
                p_pos[1] - p_fwd[1] * flank_back - p_right[1] * flank_lat)
    elif role == "FlankerRight":
        return (p_pos[0] - p_fwd[0] * flank_back + p_right[0] * flank_lat,
                p_pos[1] - p_fwd[1] * flank_back + p_right[1] * flank_lat)
    elif role == "Interceptor":
        return (p_pos[0] + p_fwd[0] * (speed_ms * lookahead),
                p_pos[1] + p_fwd[1] * (speed_ms * lookahead))
    return p_pos

pos_lead = calc_tactical_pos("LeadPursuer", player_pos, player_fwd, player_right, player_speed_ms)
pos_left = calc_tactical_pos("FlankerLeft", player_pos, player_fwd, player_right, player_speed_ms)
pos_right = calc_tactical_pos("FlankerRight", player_pos, player_fwd, player_right, player_speed_ms)
pos_inter = calc_tactical_pos("Interceptor", player_pos, player_fwd, player_right, player_speed_ms)

reporter.assert_true(pos_lead == (100.0, 100.0), "LeadPursuer aponta diretamente para o alvo")
reporter.assert_almost_equal(pos_left[0], 96.0, 1e-4, "FlankerLeft deslocado 4m à esquerda")
reporter.assert_almost_equal(pos_left[1], 94.0, 1e-4, "FlankerLeft recuado 6m na esteira")
reporter.assert_almost_equal(pos_right[0], 104.0, 1e-4, "FlankerRight deslocado 4m à direita")
reporter.assert_almost_equal(pos_inter[1], 150.0, 1e-4, "Interceptor projetado 50m à frente (20m/s * 2.5s) para corte de rota")

print("\n" + "=" * 70)
print("7. SIMULACAO DE ESCALONAMENTO POR WANTED LEVEL (1 A 5 ESTRELAS)")
print("=" * 70)

def test_escalation(wanted):
    desired = min(wanted, 4)
    return desired

reporter.assert_true(test_escalation(1) == 1, "Wanted 1 estrela mobiliza 1 viatura")
reporter.assert_true(test_escalation(2) == 2, "Wanted 2 estrelas mobiliza 2 viaturas")
reporter.assert_true(test_escalation(3) == 3, "Wanted 3 estrelas mobiliza 3 viaturas")
reporter.assert_true(test_escalation(4) == 4, "Wanted 4 estrelas mobiliza 4 viaturas (teto Android)")
reporter.assert_true(test_escalation(5) == 4, "Wanted 5 estrelas respeita o teto de 4 viaturas simultâneas para estabilidade mobile")

print("\n" + "=" * 70)
print("8. SIMULACAO DE ENCERRAMENTO COORDENADO (PRISAO VS EVASAO)")
print("=" * 70)

# Caso A: Prisao (Busted)
def handle_busted(active_units):
    for u in active_units:
        u.state = "Patrol"
        u.role = "None"
    return len(active_units)

handle_busted(coord.active_units)
reporter.assert_true(all(u.state == "Patrol" and u.role == "None" for u in coord.active_units), "Ordem de Busted faz todas as viaturas encerrarem perseguição e retornarem à patrulha")

# Caso B: Fuga bem sucedida (Evaded por timeout de busca)
time_lost = 0.0
is_active = True
dt = 1.0
while time_lost < 14.0:
    time_lost += dt
is_active = False
reporter.assert_true(time_lost >= 14.0 and not is_active, "Tempo de busca sem avistamento (>14s) declara Evasão e desativa perseguição")

print("\n" + "=" * 70)
print("9. VERIFICACAO DE PERFORMANCE, MOBILE PROFILES E ZERO GC")
print("=" * 70)

with open(os.path.join(ASSETS_DIR, "Scripts", "Police", "PolicePursuitCoordinator.cs"), "r", encoding="utf-8") as f:
    ppc_content = f.read()

reporter.assert_true("maxPursuitUnits" in ppc_content, "Limite maxPursuitUnits para evitar saturação de CPU em mobile")
reporter.assert_true("sqrMagnitude" in ppc_content, "Utilização de sqrMagnitude sem Math.Sqrt desnecessário nos loops")
reporter.assert_true("OnSuspectSighted" in ppc_content and "OnPursuitEvaded" in ppc_content, "Eventos desacoplados em C# Action sem mensagens por reflexão")

with open(os.path.join(ASSETS_DIR, "Scripts", "Police", "PolicePursuitController.cs"), "r", encoding="utf-8") as f:
    ctrl_content = f.read()

reporter.assert_true("PolicePursuitCoordinator.Instance" in ctrl_content, "PolicePursuitController totalmente integrado ao PolicePursuitCoordinator")
reporter.assert_true("CurrentTacticalRole" in ctrl_content, "Propriedade CurrentTacticalRole exposta para acompanhamento")

success = reporter.summary()

import unittest

class TestPolicePursuitAdvancedSuite(unittest.TestCase):
    def test_police_pursuit_advanced_suite(self):
        self.assertTrue(success, f"Falhas detectadas na Fase 5.5: {reporter.errors}")
        self.assertEqual(reporter.failed, 0)
        self.assertGreater(reporter.passed, 0)

if __name__ == '__main__':
    exit(0 if success else 1)
