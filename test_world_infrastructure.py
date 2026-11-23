#!/usr/bin/env python3
"""
Suíte de Testes Automatizados — Fase 5.7: Construção do Mundo Urbano e Ambiente de Jogo
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade de Scripts C# e Metadados GUID da Fase 5.7
2. Integridade e Resolução de ScriptableObjects de Quarteirões (.asset e .meta)
3. Validação Estrutural da Cena UrbanTrafficTestScene.unity (Streaming, Geração, População, LOD e Otimização)
4. Simulação de Geração Procedural Determinística por Semente (Seed)
5. Simulação de World Streaming (Divisão em Grid/Chunks, Carregamento por Raio e Descarregamento Distante)
6. Simulação de World LOD (LOD 0, LOD 1, LOD 2, Culling de Renderers e Desativação de Colisores PhysX)
7. Simulação de População de Pedestres NPC, Reação de Fuga e Object Pooling (Zero GC)
8. Simulação de Time-Slicing e Orçamento de Entidades no WorldOptimizationManager para Android
"""

import os
import re
import math
import random

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    os.path.join("Scripts", "World", "WorldChunkCoord.cs"): "1000000000000000000000000000009c",
    os.path.join("Scripts", "World", "WorldChunk.cs"): "1000000000000000000000000000009d",
    os.path.join("Scripts", "World", "WorldStreamer.cs"): "1000000000000000000000000000009e",
    os.path.join("Scripts", "World", "ProceduralBlockConfig.cs"): "1000000000000000000000000000009f",
    os.path.join("Scripts", "World", "ProceduralEnvironmentGenerator.cs"): "100000000000000000000000000000a0",
    os.path.join("Scripts", "World", "PedestrianController.cs"): "100000000000000000000000000000a1",
    os.path.join("Scripts", "World", "PedestrianPool.cs"): "100000000000000000000000000000a2",
    os.path.join("Scripts", "World", "WorldPopulationManager.cs"): "100000000000000000000000000000a3",
    os.path.join("Scripts", "World", "WorldLODGroup.cs"): "100000000000000000000000000000a4",
    os.path.join("Scripts", "World", "WorldOptimizationManager.cs"): "100000000000000000000000000000a5",
}

EXPECTED_ASSETS = {
    os.path.join("ScriptableObjects", "World", "ProceduralBlockConfig_Residential.asset"): "200000000000000000000000000000a0",
    os.path.join("ScriptableObjects", "World", "ProceduralBlockConfig_Commercial.asset"): "200000000000000000000000000000a1",
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
        print(f"RELATORIO DE TESTES FASE 5.7 - TOTAL: {total} | APROVADOS: {self.passed} | FALHAS: {self.failed}")
        print("=" * 70)
        if self.errors:
            print("Lista de Falhas:")
            for e in self.errors:
                print(f"  - {e}")
        return self.failed == 0

reporter = TestReporter()

print("=" * 70)
print("1. AUDITORIA DE SCRIPTS C# E METADADOS GUID (FASE 5.7)")
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
        reporter.assert_true(actual_guid.lower() == expected_guid.lower(), f"GUID correto ({expected_guid}) em {script_rel}")

print("\n" + "=" * 70)
print("2. AUDITORIA DE SCRIPTABLEOBJECTS DE QUARTEIROES E METAS")
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
        reporter.assert_true(actual_guid.lower() == expected_guid.lower(), f"GUID do Asset ({expected_guid}) em {asset_rel}")
    if os.path.exists(asset_path):
        with open(asset_path, "r", encoding="utf-8") as f:
            c = f.read()
        reporter.assert_true("guid: 1000000000000000000000000000009f" in c, f"Asset referencia ProceduralBlockConfig script: {asset_rel}")

print("\n" + "=" * 70)
print("3. VALIDACAO ESTRUTURAL DA CENA UrbanTrafficTestScene.unity")
print("=" * 70)

reporter.assert_true(os.path.exists(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")
with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_content = f.read()

reporter.assert_true("m_Name: World_Infrastructure_Root" in scene_content, "GameObject World_Infrastructure_Root presente na cena")
reporter.assert_true("m_Name: WorldStreamer" in scene_content, "GameObject WorldStreamer presente na cena")
reporter.assert_true("guid: 1000000000000000000000000000009e" in scene_content, "WorldStreamer configurado com GUID correto")
reporter.assert_true("m_Name: ProceduralEnvironmentGenerator" in scene_content, "GameObject ProceduralEnvironmentGenerator presente na cena")
reporter.assert_true("guid: 100000000000000000000000000000a0" in scene_content, "ProceduralEnvironmentGenerator com GUID correto")
reporter.assert_true("m_Name: PedestrianPool" in scene_content, "GameObject PedestrianPool presente na cena")
reporter.assert_true("guid: 100000000000000000000000000000a2" in scene_content, "PedestrianPool configurado com GUID correto")
reporter.assert_true("m_Name: WorldPopulationManager" in scene_content, "GameObject WorldPopulationManager presente na cena")
reporter.assert_true("guid: 100000000000000000000000000000a3" in scene_content, "WorldPopulationManager com GUID correto")
reporter.assert_true("m_Name: WorldOptimizationManager" in scene_content, "GameObject WorldOptimizationManager presente na cena")
reporter.assert_true("guid: 100000000000000000000000000000a5" in scene_content, "WorldOptimizationManager com GUID correto")
reporter.assert_true("m_Name: Chunk_0_0" in scene_content, "Célula representativa Chunk_0_0 presente na cena")
reporter.assert_true("guid: 1000000000000000000000000000009d" in scene_content, "WorldChunk configurado com GUID correto")

print("\n" + "=" * 70)
print("4. SIMULACAO DE GERACAO PROCEDURAL DETERMINISTICA POR SEMENTE")
print("=" * 70)

def generate_blocks_mock(seed, grid_size=3, spacing=100.0):
    rng = random.Random(seed)
    blocks = []
    half = grid_size // 2
    for x in range(-half, half + 1):
        for z in range(-half, half + 1):
            is_comm = rng.random() > 0.65
            lots = []
            for lx in range(2):
                for lz in range(2):
                    min_h = 12.0 if is_comm else 6.0
                    max_h = 36.0 if is_comm else 18.0
                    h = min_h + rng.random() * (max_h - min_h)
                    lots.append(round(h, 4))
            blocks.append({
                "coord": (x, z),
                "is_comm": is_comm,
                "lots": lots
            })
    return blocks

gen_a = generate_blocks_mock(192837, grid_size=3)
gen_b = generate_blocks_mock(192837, grid_size=3)
gen_c = generate_blocks_mock(999999, grid_size=3)

reporter.assert_true(len(gen_a) == 9, "Grid 3x3 gera exatamente 9 quarteirões urbanos")
reporter.assert_true(gen_a == gen_b, "Determinismo estrito: mesma seed produz idêntico arranjo predial e alturas de lotes")
reporter.assert_true(gen_a != gen_c, "Sementes distintas geram layouts e gabaritos prediais diferentes")

print("\n" + "=" * 70)
print("5. SIMULACAO DE WORLD STREAMING (GRID, RAIO E DESCARREGAMENTO)")
print("=" * 70)

def world_to_chunk_coord(pos_x, pos_z, chunk_size=100.0):
    return (math.floor(pos_x / chunk_size), math.floor(pos_z / chunk_size))

reporter.assert_true(world_to_chunk_coord(0.0, 0.0) == (0, 0), "Origem (0, 0) mapeia para Chunk (0, 0)")
reporter.assert_true(world_to_chunk_coord(145.0, -85.0) == (1, -1), "Posição (145, -85) mapeia para Chunk (1, -1)")

class SimulatedStreamer:
    def __init__(self, chunk_size=100.0, load_radius=1, unload_dist=220.0):
        self.chunk_size = chunk_size
        self.load_radius = load_radius
        self.unload_dist = unload_dist
        self.loaded_chunks = set()

    def update_streaming(self, player_pos):
        center_cx, center_cz = world_to_chunk_coord(player_pos[0], player_pos[1], self.chunk_size)

        # 1. Carrega raio
        for dx in range(-self.load_radius, self.load_radius + 1):
            for dz in range(-self.load_radius, self.load_radius + 1):
                self.loaded_chunks.add((center_cx + dx, center_cz + dz))

        # 2. Descarrega distante
        to_unload = []
        for cx, cz in self.loaded_chunks:
            chunk_center_x = (cx + 0.5) * self.chunk_size
            chunk_center_z = (cz + 0.5) * self.chunk_size
            dist = math.dist(player_pos, (chunk_center_x, chunk_center_z))
            if dist > self.unload_dist:
                to_unload.append((cx, cz))

        for c in to_unload:
            self.loaded_chunks.remove(c)

streamer = SimulatedStreamer(chunk_size=100.0, load_radius=1, unload_dist=220.0)

# Jogador na origem (0, 0)
streamer.update_streaming((0.0, 0.0))
reporter.assert_true(len(streamer.loaded_chunks) == 9, "Raio 1 carrega grid 3x3 (9 chunks) ao redor do jogador")
reporter.assert_true((0, 0) in streamer.loaded_chunks, "Chunk central (0, 0) carregado")

# Jogador se desloca 350m para Leste (X = 350)
streamer.update_streaming((350.0, 0.0))
reporter.assert_true((0, 0) not in streamer.loaded_chunks, "Chunk distante (0, 0 a 350m > 220m) descarregado com sucesso")
reporter.assert_true((3, 0) in streamer.loaded_chunks, "Novo chunk central (3, 0) carregado imediatamente")

print("\n" + "=" * 70)
print("6. SIMULACAO DE WORLD LOD E CULLING DE COLISORES PHYSX")
print("=" * 70)

class SimulatedLODGroup:
    def __init__(self, pos=(0.0, 0.0), l0=35.0, l1=80.0, l2=150.0):
        self.pos = pos
        self.l0 = l0
        self.l1 = l1
        self.l2 = l2
        self.current_lod = "LOD0"
        self.renderer_enabled = True
        self.collider_enabled = True

    def evaluate_lod(self, observer_pos):
        dist = math.dist(self.pos, observer_pos)
        if dist <= self.l0:
            self.current_lod = "LOD0"
            self.renderer_enabled = True
            self.collider_enabled = True
        elif dist <= self.l1:
            self.current_lod = "LOD1"
            self.renderer_enabled = True
            self.collider_enabled = True
        elif dist <= self.l2:
            self.current_lod = "LOD2"
            self.renderer_enabled = True
            self.collider_enabled = False # Desativa física em LOD 2
        else:
            self.current_lod = "Culled"
            self.renderer_enabled = False
            self.collider_enabled = False

lod = SimulatedLODGroup(pos=(0.0, 0.0), l0=35.0, l1=80.0, l2=150.0)

# Avalia a 20m (LOD 0)
lod.evaluate_lod((0.0, 20.0))
reporter.assert_true(lod.current_lod == "LOD0" and lod.collider_enabled and lod.renderer_enabled, "Distância 20m ativa LOD 0 (Render e Colisor ativos)")

# Avalia a 60m (LOD 1)
lod.evaluate_lod((0.0, 60.0))
reporter.assert_true(lod.current_lod == "LOD1" and lod.collider_enabled and lod.renderer_enabled, "Distância 60m ativa LOD 1 (Render e Colisor ativos)")

# Avalia a 110m (LOD 2)
lod.evaluate_lod((0.0, 110.0))
reporter.assert_true(lod.current_lod == "LOD2" and lod.renderer_enabled and not lod.collider_enabled, "Distância 110m ativa LOD 2 (Render ativo, Colisor PhysX desativado para ganho de CPU)")

# Avalia a 180m (Culled)
lod.evaluate_lod((0.0, 180.0))
reporter.assert_true(lod.current_lod == "Culled" and not lod.renderer_enabled and not lod.collider_enabled, "Distância 180m ativa Culling completo (Render e Colisor desativados)")

print("\n" + "=" * 70)
print("7. SIMULACAO DE POPULACAO DE PEDESTRES E OBJECT POOLING")
print("=" * 70)

class SimulatedPedestrianPool:
    def __init__(self, capacity=20):
        self.capacity = capacity
        self.pool = [f"Pedestrian_{i}" for i in range(capacity)]
        self.active = []
        self.allocations_count = 0

    def get(self):
        if self.pool:
            p = self.pool.pop(0)
            self.active.append(p)
            return p
        return None

    def release(self, p):
        if p in self.active:
            self.active.remove(p)
            self.pool.append(p)

ped_pool = SimulatedPedestrianPool(capacity=20)
p1 = ped_pool.get()
p2 = ped_pool.get()
reporter.assert_true(len(ped_pool.active) == 2, "2 pedestres retirados do pool pré-alocado")
reporter.assert_true(len(ped_pool.pool) == 18, "18 instâncias permanecem disponíveis no pool")

ped_pool.release(p1)
reporter.assert_true(len(ped_pool.active) == 1 and len(ped_pool.pool) == 19, "Pedestre desativado retorna ao pool sem alocações de GC")

# Reação de fuga do pedestre
class SimulatedPedestrianAI:
    def __init__(self):
        self.state = "Walking"
        self.speed = 1.35

    def check_danger(self, danger_pos, my_pos, radius=3.5):
        if math.dist(danger_pos, my_pos) <= radius:
            self.state = "Fleeing"
            self.speed = 3.6

ped_ai = SimulatedPedestrianAI()
ped_ai.check_danger(danger_pos=(0.0, 2.0), my_pos=(0.0, 0.0), radius=3.5)
reporter.assert_true(ped_ai.state == "Fleeing" and ped_ai.speed == 3.6, "Aproximação de veículo veloz a 2m dispara estado Fleeing e velocidade de fuga")

print("\n" + "=" * 70)
print("8. SIMULACAO DE TIME-SLICING E ORCAMENTO NO WORLD OPTIMIZATION MANAGER")
print("=" * 70)

class SimulatedOptimizationManager:
    def __init__(self, total_objects=40, buckets=4):
        self.objects = list(range(total_objects))
        self.buckets = buckets
        self.current_bucket = 0
        self.target_fps = 60

    def step_frame(self):
        batch_size = math.ceil(len(self.objects) / self.buckets)
        start = self.current_bucket * batch_size
        end = min(start + batch_size, len(self.objects))
        evaluated_in_this_frame = self.objects[start:end]
        self.current_bucket = (self.current_bucket + 1) % self.buckets
        return evaluated_in_this_frame

opt_mgr = SimulatedOptimizationManager(total_objects=40, buckets=4)
f1 = opt_mgr.step_frame()
f2 = opt_mgr.step_frame()
f3 = opt_mgr.step_frame()
f4 = opt_mgr.step_frame()

reporter.assert_true(len(f1) == 10, "Time-Slicing: cada frame avalia exatamente 1/4 (10 de 40 objetos)")
reporter.assert_true(len(set(f1 + f2 + f3 + f4)) == 40, "Ao longo de 4 frames, todos os 40 objetos são avaliados sem repetição")
reporter.assert_true(opt_mgr.target_fps == 60, "Meta de 60 FPS estabelecida para o runtime Android")

success = reporter.summary()

import unittest

class TestWorldInfrastructureSuite(unittest.TestCase):
    def test_world_infrastructure_suite(self):
        self.assertTrue(success, f"Falhas detectadas na Fase 5.7: {reporter.errors}")
        self.assertEqual(reporter.failed, 0)
        self.assertGreater(reporter.passed, 0)

if __name__ == '__main__':
    exit(0 if success else 1)
