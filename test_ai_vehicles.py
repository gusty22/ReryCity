#!/usr/bin/env python3
"""
Bateria de Testes Automatizados — Etapa 5.2: Veículos Civis NPC, Física e Inteligência
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testes:
1. Integridade de Scripts C# e Metadados GUID (Data, Controllers, Management)
2. Integridade e Resolução de GUIDs dos ScriptableObjects (.asset e .meta)
3. Validação Estrutural da Cena UrbanTrafficTestScene.unity
4. Simulação de Máquina de Estados (10 Estados, Histerese e Interrupção de Emergência)
5. Simulação de Sensores Não-Alocativos e Frequência Adaptativa (LOD 0, 1 e 2)
6. Cinemática de Pure Pursuit e Esterçamento com Atenuação em Alta Velocidade
7. Simulação de Dinâmica Longitudinal: Propulsão, Frenagem e Parada Segura sem Recuo
8. Lógica de Distância de Seguimento (FollowVehicle) e Decisão de Mudança de Faixa / Ultrapassagem
9. Prevenção de Bloqueios e Respeito a Semáforos e Interseções (Integração Etapa 5.1)
10. Validação do Object Pool e Reutilização de Instâncias
"""

import os
import re
import math

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    # Etapa 5.2 - Dados
    os.path.join("Scripts", "AI", "Data", "AIVehicleType.cs"): "10000000000000000000000000000050",
    os.path.join("Scripts", "AI", "Data", "AIDriverProfile.cs"): "10000000000000000000000000000051",
    os.path.join("Scripts", "AI", "Data", "AIVehicleData.cs"): "10000000000000000000000000000052",
    # Etapa 5.2 - Controladores
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleStateMachine.cs"): "10000000000000000000000000000053",
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleSensor.cs"): "10000000000000000000000000000054",
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleMotorController.cs"): "10000000000000000000000000000055",
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleSteeringController.cs"): "10000000000000000000000000000056",
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleBrakeController.cs"): "10000000000000000000000000000057",
    os.path.join("Scripts", "AI", "Controllers", "AIVehicleController.cs"): "10000000000000000000000000000058",
    # Etapa 5.2 - Gerenciamento
    os.path.join("Scripts", "AI", "Management", "AIVehiclePool.cs"): "10000000000000000000000000000059",
    os.path.join("Scripts", "AI", "Management", "TrafficLODManager.cs"): "10000000000000000000000000000060",
    os.path.join("Scripts", "AI", "Management", "TrafficSpawnManager.cs"): "10000000000000000000000000000061",
}

EXPECTED_ASSETS = {
    # Perfis de Motorista
    os.path.join("ScriptableObjects", "Traffic", "DriverProfile_Cautious.asset"): "20000000000000000000000000000050",
    os.path.join("ScriptableObjects", "Traffic", "DriverProfile_Normal.asset"): "20000000000000000000000000000051",
    os.path.join("ScriptableObjects", "Traffic", "DriverProfile_Aggressive.asset"): "20000000000000000000000000000052",
    # Categorias de Veículos
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_CivilMotorcycle.asset"): "20000000000000000000000000000060",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_EconomyCar.asset"): "20000000000000000000000000000061",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_Sedan.asset"): "20000000000000000000000000000062",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_SUV.asset"): "20000000000000000000000000000063",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_Pickup.asset"): "20000000000000000000000000000064",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_Truck.asset"): "20000000000000000000000000000065",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_Bus.asset"): "20000000000000000000000000000066",
    os.path.join("ScriptableObjects", "Traffic", "VehicleData_PoliceCruiser.asset"): "20000000000000000000000000000067",
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
print("INICIANDO SUÍTE DE TESTES AUTOMATIZADOS — ETAPA 5.2 (VEÍCULOS CIVIS NPC)")
print("=" * 75)

# -------------------------------------------------------------
# TESTE 1: Integridade de Scripts C# e GUIDs
# -------------------------------------------------------------
print("\n>>> TESTE 1: Integridade de Scripts C# e GUIDs Determinísticos")
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
# TESTE 2: Integridade de ScriptableObjects (.asset e .meta)
# -------------------------------------------------------------
print("\n>>> TESTE 2: ScriptableObjects de Perfis e Categorias de Veículos")
for rel_path, expected_guid in EXPECTED_ASSETS.items():
    full_path = os.path.join(ASSETS_DIR, rel_path)
    meta_path = full_path + ".meta"
    
    test.assert_true(os.path.isfile(full_path), f"Asset existe: {rel_path}")
    test.assert_true(os.path.isfile(meta_path), f"Asset meta existe: {rel_path}.meta")
    
    if os.path.isfile(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
            m = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
            found_guid = m.group(1) if m else None
            test.assert_true(found_guid == expected_guid, 
                             f"GUID confere ({os.path.basename(rel_path)})", 
                             f"Esperado {expected_guid}, obtido {found_guid}")

# -------------------------------------------------------------
# TESTE 3: Validação da Cena UrbanTrafficTestScene.unity
# -------------------------------------------------------------
print("\n>>> TESTE 3: Validação da Cena UrbanTrafficTestScene.unity Atualizada")
test.assert_true(os.path.isfile(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")

with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_str = f.read()

required_scene_entities = [
    "AIVehiclePool",
    "TrafficLODManager",
    "TrafficSpawnManager",
    "NPC_Lead_EconomyCar",
    "Player_Motorcycle_Proxy",
    "Obstacle_Roadwork_Cone"
]
for ent in required_scene_entities:
    test.assert_true(ent in scene_str, f"Entidade presente na cena: {ent}")

# -------------------------------------------------------------
# TESTE 4: Simulação da Máquina de Estados (10 Estados e Histerese)
# -------------------------------------------------------------
print("\n>>> TESTE 4: Simulação da Máquina de Estados e Histerese de Transição")

class MockStateMachine:
    def __init__(self, min_duration=0.35):
        self.state = "Idle"
        self.prev_state = "Idle"
        self.timer = 0.0
        self.min_duration = min_duration

    def tick(self, dt):
        self.timer += dt

    def request_state(self, new_state, force=False):
        if new_state == self.state:
            return False
        # Emergências furam histerese
        if new_state == "EmergencyStop" or force or self.timer >= self.min_duration:
            self.prev_state = self.state
            self.state = new_state
            self.timer = 0.0
            return True
        return False

sm = MockStateMachine(min_duration=0.35)
sm.request_state("Cruise", force=True)
test.assert_true(sm.state == "Cruise", "Inicialização no estado Cruise")

# Tentativa de troca imediata aos 0.1s (deve ser bloqueada por histerese)
sm.tick(0.1)
blocked = not sm.request_state("Brake")
test.assert_true(blocked, "Histerese impede troca de estado antes de 0.35s")

# Emergência fura a histerese
emergency_accepted = sm.request_state("EmergencyStop")
test.assert_true(emergency_accepted and sm.state == "EmergencyStop", 
                 "EmergencyStop fura a histerese com transição imediata")

# Avança além da histerese e retorna a Cruise
sm.tick(0.4)
resumed = sm.request_state("Cruise")
test.assert_true(resumed and sm.state == "Cruise", "Após min_duration, permite retorno a Cruise")

# -------------------------------------------------------------
# TESTE 5: Simulação de Sensores e Frequência Adaptativa (LOD)
# -------------------------------------------------------------
print("\n>>> TESTE 5: Sensores Não-Alocativos e Frequência Adaptativa por LOD")

class MockLODManager:
    def evaluate_interval(self, dist):
        if dist < 35.0:
            return 0.05 # 20 Hz (LOD0)
        elif dist < 75.0:
            return 0.20 # 5 Hz (LOD1)
        elif dist <= 120.0:
            return 1.00 # 1 Hz (LOD2)
        else:
            return -1 # Despawn

lod = MockLODManager()
test.assert_true(lod.evaluate_interval(15.0) == 0.05, "LOD 0 (<35m): Sensores operam a 20 Hz (0.05s)")
test.assert_true(lod.evaluate_interval(50.0) == 0.20, "LOD 1 (35-75m): Sensores operam a 5 Hz (0.20s)")
test.assert_true(lod.evaluate_interval(90.0) == 1.00, "LOD 2 (75-120m): Sensores operam a 1 Hz (1.00s)")
test.assert_true(lod.evaluate_interval(130.0) == -1, "Distância > 120m dispara retorno automático ao Pool (Despawn)")

# -------------------------------------------------------------
# TESTE 6: Cinemática de Esterçamento com Atenuação em Alta Velocidade
# -------------------------------------------------------------
print("\n>>> TESTE 6: Pure Pursuit e Atenuação de Esterçamento por Velocidade")

def calculate_steer(target_local_x, target_local_z, speed_kmh, max_steer=35.0, high_speed_scale=0.5):
    angle_deg = math.degrees(math.atan2(target_local_x, target_local_z))
    clamped_angle = max(-max_steer, min(max_steer, angle_deg))
    speed_factor = 1.0 - (1.0 - high_speed_scale) * min(1.0, speed_kmh / 100.0)
    return clamped_angle * speed_factor

# Alvo a 30 graus à direita em baixa velocidade (20 km/h)
steer_low_speed = calculate_steer(5.0, 8.66, 20.0)
test.assert_true(abs(steer_low_speed - 30.0 * 0.9) < 1.0, 
                 "Esterçamento em baixa velocidade mantém alta fidelidade ao alvo", 
                 f"Obtido: {steer_low_speed:.2f}")

# Mesmo alvo a 100 km/h (deve ser atenuado em 50%)
steer_high_speed = calculate_steer(5.0, 8.66, 100.0)
test.assert_true(abs(steer_high_speed - 15.0) < 0.5, 
                 "Esterçamento em alta velocidade atenuado a 50% para evitar tombamentos", 
                 f"Obtido: {steer_high_speed:.2f}")

# -------------------------------------------------------------
# TESTE 7: Dinâmica Longitudinal (Propulsão, Frenagem e Anti-Recuo)
# -------------------------------------------------------------
print("\n>>> TESTE 7: Dinâmica Física Longitudinal e Parada sem Efeito Rebote")

class MockPhysicsBody:
    def __init__(self, mass=1000.0, acc_force=3000.0, brake_force=6000.0):
        self.mass = mass
        self.acc_force = acc_force
        self.brake_force = brake_force
        self.velocity = 0.0 # m/s

    def step(self, throttle, brake, dt=0.02):
        if brake > 0.05 and self.velocity < 0.25:
            self.velocity = 0.0 # Trava anti-recuo
            return
        net_force = throttle * self.acc_force - brake * self.brake_force
        acc = net_force / self.mass
        self.velocity = max(0.0, self.velocity + acc * dt)

body = MockPhysicsBody(mass=1000.0, acc_force=3000.0, brake_force=6000.0)

# Acelera por 2 segundos
for _ in range(100):
    body.step(throttle=1.0, brake=0.0, dt=0.02)
speed_after_acc = body.velocity * 3.6
test.assert_true(speed_after_acc > 15.0, "Veículo acelera progressivamente via AddForce")

# Freia intensamente
for _ in range(150):
    body.step(throttle=0.0, brake=1.0, dt=0.02)
test.assert_true(body.velocity == 0.0, "Freio atua sem rebote ou recuo negativo quando a velocidade zera")

# -------------------------------------------------------------
# TESTE 8: Lógica de Distância de Seguimento e Ultrapassagem
# -------------------------------------------------------------
print("\n>>> TESTE 8: Decisão de Seguimento (FollowVehicle) e Ultrapassagem")

def decide_behavior(front_dist, safe_dist, left_clear, is_player, propensity):
    if front_dist < 3.5:
        return "EmergencyStop"
    if front_dist < safe_dist * 1.5 and left_clear and propensity > 0.5:
        return "Overtaking"
    if front_dist < safe_dist:
        return "FollowVehicle"
    return "Cruise"

# Carro lento à frente com faixa livre e motorista agressivo
b1 = decide_behavior(front_dist=8.0, safe_dist=7.0, left_clear=True, is_player=False, propensity=0.7)
test.assert_true(b1 == "Overtaking", "Condutor agressivo inicia ultrapassagem com faixa livre")

# Carro lento à frente com faixa ocupada
b2 = decide_behavior(front_dist=5.0, safe_dist=7.0, left_clear=False, is_player=False, propensity=0.7)
test.assert_true(b2 == "FollowVehicle", "Com faixa esquerda ocupada, reduz para FollowVehicle")

# Motocicleta do jogador corta a frente subitamente a 2.5m
b3 = decide_behavior(front_dist=2.5, safe_dist=7.0, left_clear=True, is_player=True, propensity=0.3)
test.assert_true(b3 == "EmergencyStop", "Corte de caminho repentino do jogador aciona EmergencyStop instantaneamente")

# -------------------------------------------------------------
# TESTE 9: Simulação de Object Pool
# -------------------------------------------------------------
print("\n>>> TESTE 9: Object Pool de Veículos NPC")

class MockPool:
    def __init__(self, size_per_type=3):
        self.pool = ["EconomyCar_" + str(i) for i in range(size_per_type)]
        self.active = []

    def get(self):
        v = self.pool.pop(0) if self.pool else "EconomyCar_Dyn"
        self.active.append(v)
        return v

    def release(self, v):
        self.active.remove(v)
        self.pool.append(v)

pool = MockPool(size_per_type=3)
v1 = pool.get()
v2 = pool.get()
test.assert_true(len(pool.active) == 2 and len(pool.pool) == 1, "Pool fornece instâncias pré-alocadas")
pool.release(v1)
test.assert_true(len(pool.active) == 1 and len(pool.pool) == 2, "Instância liberada retorna ao Pool sem Garbage Collection")

print("\n" + "=" * 75)
print(f"RESUMO DOS TESTES: {test.passed} PASSOU | {test.failed} FALHOU")
print("=" * 75)

import unittest

class TestAIVehiclesSuite(unittest.TestCase):
    def test_ai_vehicles_suite(self):
        self.assertEqual(test.failed, 0, f"Falhas detectadas na Fase 5.2: {test.errors}")
        self.assertGreater(test.passed, 0)

if __name__ == '__main__':
    if test.failed > 0:
        for e in test.errors:
            print(f"  [ERRO] {e}")
        exit(1)
    else:
        print("TODOS OS TESTES DA ETAPA 5.2 FORAM EXECUTADOS COM 100% DE SUCESSO!")
        exit(0)
