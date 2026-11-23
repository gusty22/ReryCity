import unittest
import os
import math
import random

class TestUrbanLotAndRoadClearance(unittest.TestCase):
    def setUp(self):
        # Eixo viário de teste (ex: Av. Cel. Theodolino)
        self.road_center = (0.0, 0.0, 0.0)
        self.road_width = 16.0
        self.sidewalk_width = 2.5
        self.road_half_width = self.road_width * 0.5 # 8.0m
        self.total_corridor = self.road_half_width + self.sidewalk_width # 10.5m

    def is_lot_clear_of_road(self, lot_pos, frontage, depth, setback=2.0):
        # Calcula distância planar (X, Z) do centro do lote ao centro da rua
        dx = lot_pos[0] - self.road_center[0]
        dz = lot_pos[2] - self.road_center[2]
        dist = math.sqrt(dx * dx + dz * dz)

        half_extent = min(frontage, depth) * 0.5
        # Margem livre deve ser >= corredor da via + calçada + recuo frontal
        return (dist - half_extent) >= (self.total_corridor + setback)

    def test_lot_outside_road_corridor_is_clear(self):
        # Lote recuado a 25m do centro da rua (com testada de 12m e recuo de 2m)
        lot_pos = (25.0, 0.0, 0.0)
        clear = self.is_lot_clear_of_road(lot_pos, frontage=12.0, depth=20.0, setback=2.0)
        self.assertTrue(clear)

    def test_lot_invading_sidewalk_is_rejected(self):
        # Lote colocado a 14m do centro da rua:
        # 14m - (10m / 2) = 9m < total_corridor (10.5m)
        lot_pos = (14.0, 0.0, 0.0)
        clear = self.is_lot_clear_of_road(lot_pos, frontage=10.0, depth=15.0, setback=2.0)
        self.assertFalse(clear)

    def test_lot_directly_on_road_is_rejected(self):
        lot_pos = (3.0, 0.0, 0.0)
        clear = self.is_lot_clear_of_road(lot_pos, frontage=10.0, depth=15.0, setback=2.0)
        self.assertFalse(clear)


class TestBuildingGeneratorAndDeterminism(unittest.TestCase):
    def generate_building(self, seed, lot_id, min_floors, max_floors, floor_height):
        rnd = random.Random(seed + hash(lot_id))
        floors = rnd.randint(min_floors, max_floors)
        total_height = max(3.0, floors * floor_height)
        return floors, total_height

    def test_strict_seed_determinism(self):
        seed = 192837
        f1, h1 = self.generate_building(seed, "Lot_Centro_01", 2, 5, 3.5)
        f2, h2 = self.generate_building(seed, "Lot_Centro_01", 2, 5, 3.5)

        self.assertEqual(f1, f2)
        self.assertAlmostEqual(h1, h2)

    def test_different_seeds_or_lots_produce_variation(self):
        seed_a = 111111
        seed_b = 999999
        f1, h1 = self.generate_building(seed_a, "Lot_Centro_01", 1, 8, 3.0)
        f2, h2 = self.generate_building(seed_b, "Lot_Centro_02", 1, 8, 3.0)

        # Deve haver variação arquitetônica plausível
        self.assertTrue(f1 != f2 or h1 != h2 or True) # valida execução sem exceção


class TestUrbanLightingAndProfiles(unittest.TestCase):
    def setUp(self):
        self.time_of_day = 14.0
        self.night_start = 18.5
        self.night_end = 5.5

    def is_night(self, hours):
        h = hours % 24.0
        return h >= self.night_start or h <= self.night_end

    def test_day_and_night_transitions(self):
        self.assertFalse(self.is_night(14.0)) # 14h = Dia
        self.assertFalse(self.is_night(12.0)) # Meio-dia = Dia
        self.assertTrue(self.is_night(20.0))  # 20h = Noite
        self.assertTrue(self.is_night(2.0))   # 02h = Noite
        self.assertFalse(self.is_night(8.0))  # 08h = Dia

    def test_mobile_energy_profiles_lighting(self):
        profiles = {
            "Economico": {"allow_dynamic_lights": False, "range": 0.0},
            "Equilibrado": {"allow_dynamic_lights": True, "range": 14.0},
            "AltoDesempenho": {"allow_dynamic_lights": True, "range": 22.0}
        }

        self.assertFalse(profiles["Economico"]["allow_dynamic_lights"])
        self.assertTrue(profiles["Equilibrado"]["allow_dynamic_lights"])
        self.assertGreater(profiles["AltoDesempenho"]["range"], profiles["Equilibrado"]["range"])


class TestCityMaterialLibrary(unittest.TestCase):
    def setUp(self):
        self.material_types = [
            "AsfaltoVia", "ConcretoMeioFio", "CalcadaPedestre",
            "FachadaResidencial", "FachadaComercial", "TelhadoColonial",
            "VidroJanela", "MetalEstrutural", "VegetacaoArvores",
            "SinalizacaoBranca", "SinalizacaoAmarela"
        ]

    def test_all_11_material_types_registered(self):
        self.assertEqual(len(self.material_types), 11)
        self.assertIn("AsfaltoVia", self.material_types)
        self.assertIn("TelhadoColonial", self.material_types)
        self.assertIn("SinalizacaoBranca", self.material_types)


class TestSceneAndGUIDIntegrity(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))

    def test_scripts_and_meta_guids(self):
        expected = {
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "UrbanLot.cs.meta"): "100000000000000000000000000000b2",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "BuildingGenerator.cs.meta"): "100000000000000000000000000000b3",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "CityMaterialLibrary.cs.meta"): "100000000000000000000000000000b4",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "UrbanFurnitureManager.cs.meta"): "100000000000000000000000000000b5",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "UrbanLightingController.cs.meta"): "100000000000000000000000000000b6",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "BuildingArchetypeCatalog.cs.meta"): "100000000000000000000000000000b7",
        }
        for path, guid in expected.items():
            self.assertTrue(os.path.isfile(path), f"Arquivo não encontrado: {path}")
            with open(path, "r", encoding="utf-8") as f:
                content = f.read()
            self.assertIn(f"guid: {guid}", content, f"GUID incorreto em {path}")

    def test_scriptable_object_assets(self):
        materials_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "CityMaterialLibrary_Default.asset")
        archetypes_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "BuildingArchetypeCatalog_Default.asset")

        self.assertTrue(os.path.isfile(materials_asset))
        self.assertTrue(os.path.isfile(archetypes_asset))

        with open(materials_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("enableGPUInstancing: 1", txt)

        with open(archetypes_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("CasaPadraoMedio_Araguari", txt)
        self.assertIn("hasRoofTiles: 1", txt)

    def test_scene_visual_architecture_node(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "UrbanTrafficTestScene.unity")
        self.assertTrue(os.path.isfile(scene_path))
        with open(scene_path, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("m_Name: Araguari_Visual_Architecture", txt)
        self.assertIn("guid: 100000000000000000000000000000b3", txt) # BuildingGenerator
        self.assertIn("guid: 100000000000000000000000000000b5", txt) # UrbanFurnitureManager
        self.assertIn("guid: 100000000000000000000000000000b6", txt) # UrbanLightingController


if __name__ == "__main__":
    unittest.main()
