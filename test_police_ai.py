#!/usr/bin/env python3
"""
Suíte de Testes Automatizados — Fase 5.3: Inteligência Artificial Policial e Sistema de Procurado
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade de Scripts C# e Metadados GUID da Fase 5.3
2. Integridade e Resolução de GUIDs dos ScriptableObjects de Polícia (.asset e .meta)
3. Validação Estrutural da Cena UrbanTrafficTestScene.unity (Entidades Policiais e HUD)
4. Simulação da Máquina de Estados Policial (8 Estados, Histerese e Prioridades)
5. Simulação do Detector de Infrações de Trânsito (Tipos, Pesos e Cooldowns)
6. Simulação do WantedLevelManager (Escalonamento 0 a 5 Estrelas e Decay de Evasão)
7. Simulação de Percepção: Cone de Visão, Perda de Contato e Busca na Última Posição Conhecida
8. Simulação de Tática de Interceptação com Predição Linear de Rota
9. Simulação de Abordagem, Voz de Prisão (Busted) e Fuga com Recidiva
"""

import os
import re
import math

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    os.path.join("Scripts", "Police", "PoliceVehicleData.cs"): "10000000000000000000000000000071",
    os.path.join("Scripts", "Police", "PoliceAIData.cs"): "10000000000000000000000000000072",
    os.path.join("Scripts", "Police", "PoliceAIStateMachine.cs"): "10000000000000000000000000000073",
    os.path.join("Scripts", "Police", "TrafficViolationDetector.cs"): "10000000000000000000000000000074",
    os.path.join("Scripts", "Police", "WantedLevelManager.cs"): "10000000000000000000000000000075",
    os.path.join("Scripts", "Police", "PolicePursuitController.cs"): "10000000000000000000000000000076",
    os.path.join("Scripts", "UI", "PoliceHUD.cs"): "10000000000000000000000000000077",
    os.path.join("Scripts", "UI", "WantedLevelUI.cs"): "10000000000000000000000000000078",
}

EXPECTED_ASSETS = {
    os.path.join("ScriptableObjects", "Police", "PoliceVehicleData_ViaturaHilux.asset"): "20000000000000000000000000000080",
    os.path.join("ScriptableObjects", "Police", "PoliceVehicleData_RocamTiger800.asset"): "20000000000000000000000000000081",
    os.path.join("ScriptableObjects", "Police", "PoliceAIData_StandardPatrol.asset"): "20000000000000000000000000000082",
    os.path.join("ScriptableObjects", "Police", "PoliceAIData_RocamPursuit.asset"): "20000000000000000000000000000083",
}

class TestReporter:
    def __init__(self):
        self.passed = 0
        self.failed = 0
        self.errors = []

    def assert_true(self, condition, name, details=""):
        if condition:
            self.passed += 1
            print(f"  [PASS] {name}")
        else:
            self.failed += 1
            err = f"{name} FAILED: {details}"
            self.errors.append(err)
            print(f"  [FAIL] {err}")

test = TestReporter()

print("=" * 75)
print("INICIANDO SUÍTE DE TESTES AUTOMATIZADOS — FASE 5.3 (IA POLICIAL & PROCURADO)")
print("=" * 75)

# -------------------------------------------------------------
# TESTE 1: Integridade de Scripts C# e GUIDs
# -------------------------------------------------------------
print("\n>>> TESTE 1: Integridade de Scripts C# e GUIDs da Fase 5.3")
for rel_path, expected_guid in EXPECTED_SCRIPTS.items():
    full_path = os.path.join(ASSETS_DIR, rel_path)
    meta_path = full_path + ".meta"
    
    test.assert_true(os.path.isfile(full_path), f"Arquivo de script existe: {rel_path}")
    test.assert_true(os.path.isfile(meta_path), f"Meta existe: {rel_path}.meta")
    
    if os.path.isfile(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
            m = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
            found_guid = m.group(1) if m else None
            test.assert_true(found_guid == expected_guid, 
                             f"GUID confere ({os.path.basename(rel_path)})", 
                             f"Esperado {expected_guid}, encontrado {found_guid}")

# -------------------------------------------------------------
# TESTE 2: Integridade de ScriptableObjects Policiais
# -------------------------------------------------------------
print("\n>>> TESTE 2: ScriptableObjects de Viaturas e IA Policial")
for rel_path, expected_guid in EXPECTED_ASSETS.items():
    full_path = os.path.join(ASSETS_DIR, rel_path)
    meta_path = full_path + ".meta"
    
    test.assert_true(os.path.isfile(full_path), f"Asset existe: {rel_path}")
    test.assert_true(os.path.isfile(meta_path), f"Meta existe: {rel_path}.meta")
    
    if os.path.isfile(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
            m = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
            found_guid = m.group(1) if m else None
            test.assert_true(found_guid == expected_guid, 
                             f"GUID confere ({os.path.basename(rel_path)})", 
                             f"Esperado {expected_guid}, encontrado {found_guid}")

# -------------------------------------------------------------
# TESTE 3: Validação da Cena UrbanTrafficTestScene.unity
# -------------------------------------------------------------
print("\n>>> TESTE 3: Presença de Entidades Policiais e HUD na Cena")
test.assert_true(os.path.isfile(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")

with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_str = f.read()

required_police_entities = [
    "TrafficViolationDetector",
    "WantedLevelManager",
    "Police_UI_Canvas",
    "Police_Cruiser_Hilux",
    "Police_ROCAM_Tiger800"
]
for ent in required_police_entities:
    test.assert_true(ent in scene_str, f"Entidade presente na cena: {ent}")

# -------------------------------------------------------------
# TESTE 4: Simulação da Máquina de Estados Policial (8 Estados)
# -------------------------------------------------------------
print("\n>>> TESTE 4: Máquina de 8 Estados Policiais e Histerese")

class MockPoliceStateMachine:
    def __init__(self, min_duration=0.4):
        self.state = "Patrol"
        self.prev_state = "Patrol"
        self.timer = 0.0
        self.min_duration = min_duration

    def tick(self, dt):
        self.timer += dt

    def request_state(self, new_state, force=False):
        if new_state == self.state:
            return False
        # Pursuit, Intercepting e Arresting têm prioridade e furam histerese
        high_priority = new_state in ("Pursuit", "Intercepting", "Arresting")
        if high_priority or force or self.timer >= self.min_duration:
            self.prev_state = self.state
            self.state = new_state
            self.timer = 0.0
            return True
        return False

psm = MockPoliceStateMachine(min_duration=0.4)
test.assert_true(psm.state == "Patrol", "Inicia em estado Patrol")

# Transição para Suspicious antes do tempo (bloqueada)
psm.tick(0.1)
test.assert_true(not psm.request_state("Suspicious"), "Histerese bloqueia transição de rotina antes de 0.4s")

# Disparo de perseguição direta (alta prioridade fura histerese)
test.assert_true(psm.request_state("Pursuit"), "Pursuit fura histerese com transição imediata")
test.assert_true(psm.state == "Pursuit", "Estado agora é Pursuit")

# Interceptação também fura histerese
test.assert_true(psm.request_state("Intercepting"), "Intercepting aceito prioritariamente")
test.assert_true(psm.state == "Intercepting", "Estado agora é Intercepting")

# LostTarget respeita histerese
psm.tick(0.1)
test.assert_true(not psm.request_state("LostTarget"), "LostTarget aguarda histerese")
psm.tick(0.35)
test.assert_true(psm.request_state("LostTarget"), "Após histerese, permite transição para LostTarget")

# -------------------------------------------------------------
# TESTE 5: Simulação do Detector de Infrações e Cooldowns
# -------------------------------------------------------------
print("\n>>> TESTE 5: Detecção de Infrações, Pontos e Cooldowns")

class MockViolationDetector:
    def __init__(self):
        self.cooldowns = {
            "Speeding": 0.0,
            "WrongWay": 0.0,
            "RecklessWheelie": 0.0,
            "RedLight": 0.0,
            "Evading": 0.0
        }
        self.events = []

    def tick(self, dt):
        for k in self.cooldowns:
            if self.cooldowns[k] > 0.0:
                self.cooldowns[k] -= dt

    def report_infraction(self, infraction, score, cooldown):
        if self.cooldowns[infraction] <= 0.0:
            self.cooldowns[infraction] = cooldown
            self.events.append((infraction, score))
            return True
        return False

vd = MockViolationDetector()
# 1. Comete excesso de velocidade
r1 = vd.report_infraction("Speeding", 15, cooldown=4.0)
test.assert_true(r1 == True, "Infração de Speeding registrada na primeira ocorrência")

# 2. No frame seguinte, tenta registrar a mesma infração (deve falhar por cooldown)
vd.tick(0.02)
r2 = vd.report_infraction("Speeding", 15, cooldown=4.0)
test.assert_true(r2 == False, "Cooldown impede duplicação de infração no frame subsequente")

# 3. Comete infração de grau (tipo diferente, cooldown independente)
r3 = vd.report_infraction("RecklessWheelie", 35, cooldown=3.0)
test.assert_true(r3 == True, "Infrações de naturezas distintas possuem cooldowns independentes")

# 4. Avança 4.1s (expira cooldown de Speeding)
vd.tick(4.1)
r4 = vd.report_infraction("Speeding", 15, cooldown=4.0)
test.assert_true(r4 == True, "Após expirar o cooldown, nova infração de velocidade é permitida")

# -------------------------------------------------------------
# TESTE 6: WantedLevelManager (Escalonamento e Evasão)
# -------------------------------------------------------------
print("\n>>> TESTE 6: WantedLevelManager — Progressão e Decay de Evasão")

class MockWantedManager:
    def __init__(self):
        self.score = 0
        self.level = 0
        self.is_spotted = False
        self.evasion_timer = 0.0
        self.evasion_target = 15.0

    def add_score(self, pts):
        self.score += pts
        self.evasion_timer = 0.0
        self._eval_level()

    def _eval_level(self):
        if self.score >= 450: self.level = 5
        elif self.score >= 250: self.level = 4
        elif self.score >= 130: self.level = 3
        elif self.score >= 60: self.level = 2
        elif self.score >= 20: self.level = 1
        else: self.level = 0

    def tick_evasion(self, dt):
        if self.level > 0 and not self.is_spotted:
            self.evasion_timer += dt
            if self.evasion_timer >= self.evasion_target:
                self.evasion_timer = 0.0
                self.level -= 1
                thresholds = {5: 450, 4: 250, 3: 130, 2: 60, 1: 20, 0: 0}
                self.score = thresholds.get(self.level, 0)

wm = MockWantedManager()
test.assert_true(wm.level == 0, "Inicia com Nível 0 (Sem Procurado)")

# Adiciona infração leve (15 pts) -> continua 0
wm.add_score(15)
test.assert_true(wm.level == 0, "15 pts < threshold 20 -> Permanece nível 0")

# Adiciona mais 15 pts (total 30 pts) -> sobe para Nível 1
wm.add_score(15)
test.assert_true(wm.level == 1, "30 pts >= 20 -> Sobe para Nível 1 (Infração Identificada)")

# Comete manobras e sobe para 140 pts -> Nível 3 (Perseguição Ativa)
wm.add_score(110)
test.assert_true(wm.level == 3, "140 pts >= 130 -> Sobe para Nível 3 (Perseguição Ativa)")

# Simula evasão: viaturas perdem contato visual (is_spotted = False)
wm.is_spotted = False
wm.tick_evasion(10.0) # 10s ainda não atinge 15s
test.assert_true(wm.level == 3, "Aos 10s de evasão, permanece no nível 3")

wm.tick_evasion(5.5) # Total > 15s -> reduz 1 estrela
test.assert_true(wm.level == 2, "Aos 15.5s de evasão bem-sucedida, decresce para Nível 2")

# Se for avistado novamente, o timer de evasão é zerado imediatamente
wm.is_spotted = True
wm.tick_evasion(10.0)
test.assert_true(wm.evasion_timer == 0.0 and wm.level == 2, "Avistamento imediato cancela contagem de evasão")

# -------------------------------------------------------------
# TESTE 7: Percepção Sensorial e Busca na Última Posição Conhecida
# -------------------------------------------------------------
print("\n>>> TESTE 7: Percepção (Cone de Visão e Varredura na LKP)")

def check_vision(police_pos, police_forward, target_pos, max_range=55.0, fov=110.0):
    to_target = (target_pos[0]-police_pos[0], target_pos[2]-police_pos[2])
    dist = math.hypot(to_target[0], to_target[1])
    if dist > max_range: return False
    
    # Produto escalar para calcular ângulo
    dot = (police_forward[0]*to_target[0] + police_forward[1]*to_target[1]) / dist
    angle = math.degrees(math.acos(max(-1.0, min(1.0, dot))))
    return angle <= fov * 0.5

# Alvo a 30m diretamente à frente (ângulo 0°)
v1 = check_vision((0, 0, 0), (0, 1), (0, 0, 30))
test.assert_true(v1 == True, "Alvo dentro do alcance frontal é avistado")

# Alvo a 40m com 45° de deflexão (dentro de 55° metade de FOV 110°)
v2 = check_vision((0, 0, 0), (0, 1), (28.28, 0, 28.28))
test.assert_true(v2 == True, "Alvo lateral dentro do cone de visão é avistado")

# Alvo a 30m atrás da viatura (ângulo 180°)
v3 = check_vision((0, 0, 0), (0, 1), (0, 0, -30))
test.assert_true(v3 == False, "Alvo na traseira fora do cone de visão NÃO é avistado")

# -------------------------------------------------------------
# TESTE 8: Tática de Interceptação com Predição de Rota Futura
# -------------------------------------------------------------
print("\n>>> TESTE 8: Interceptação Tática com Predição de Trajetória")

def predict_intercept_point(player_pos, player_forward, player_speed_kmh, look_ahead_time=1.8):
    speed_ms = player_speed_kmh / 3.6
    future_x = player_pos[0] + player_forward[0] * speed_ms * look_ahead_time
    future_z = player_pos[2] + player_forward[2] * speed_ms * look_ahead_time
    return (future_x, 0.0, future_z)

# Moto a 72 km/h (20 m/s) se movendo em Z positivo
predicted = predict_intercept_point((0.0, 0.0, 10.0), (0.0, 0.0, 1.0), 72.0, look_ahead_time=1.8)
expected_z = 10 + 20 * 1.8 # 46.0m
test.assert_true(abs(predicted[2] - expected_z) < 0.1, 
                 "Ponto de interceptação calculado com predição precisa de trajetória",
                 f"Esperado Z={expected_z}, obtido Z={predicted[2]}")

# -------------------------------------------------------------
# TESTE 9: Abordagem e Prisão (Busted) vs Fuga
# -------------------------------------------------------------
print("\n>>> TESTE 9: Abordagem, Ordem de Parada e Prisão Consumada")

class MockArrestSystem:
    def __init__(self, arrest_duration=3.0, max_speed_kmh=8.0):
        self.arrest_timer = 0.0
        self.arrest_duration = arrest_duration
        self.max_speed = max_speed_kmh
        self.busted = False

    def update_arrest(self, distance, player_speed, dt):
        if distance <= 4.5 and player_speed <= self.max_speed:
            self.arrest_timer += dt
            if self.arrest_timer >= self.arrest_duration:
                self.busted = True
                return "Busted"
            return "Arresting"
        elif self.arrest_timer > 0.0 and player_speed > self.max_speed:
            # Jogador fugiu durante abordagem
            self.arrest_timer = 0.0
            return "Evading"
        return "Pursuing"

arr = MockArrestSystem(arrest_duration=3.0, max_speed_kmh=8.0)

# Moto para ao lado da viatura por 1.5s
st1 = arr.update_arrest(distance=3.5, player_speed=2.0, dt=1.5)
test.assert_true(st1 == "Arresting", "Viatura inicia contagem de abordagem ao cercar moto parada")

# Moto acelera para fugir da viatura aos 1.5s
st2 = arr.update_arrest(distance=3.5, player_speed=40.0, dt=0.1)
test.assert_true(st2 == "Evading", "Aceleração durante abordagem cancela contagem e aciona Evading")

# Nova abordagem: moto permanece imobilizada por 3.2s
arr.update_arrest(distance=3.0, player_speed=0.0, dt=1.5)
st3 = arr.update_arrest(distance=3.0, player_speed=0.0, dt=1.8)
test.assert_true(st3 == "Busted" and arr.busted, "Permanência imobilizada consome o tempo necessário e consuma a Prisão (Busted)")

print("\n" + "=" * 75)
print(f"RESUMO DOS TESTES: {test.passed} PASSOU | {test.failed} FALHOU")
print("=" * 75)

import unittest

class TestPoliceAISuite(unittest.TestCase):
    def test_police_ai_suite(self):
        self.assertEqual(test.failed, 0, f"Falhas detectadas na Fase 5.3: {test.errors}")
        self.assertGreater(test.passed, 0)

if __name__ == '__main__':
    if test.failed > 0:
        for e in test.errors:
            print(f"  [ERRO] {e}")
        exit(1)
    else:
        print("TODOS OS TESTES DA FASE 5.3 FORAM CONCLUÍDOS COM 100% DE SUCESSO!")
        exit(0)
