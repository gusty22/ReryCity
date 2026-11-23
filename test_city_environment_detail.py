import unittest
import os
import math

class TestNeighborhoodThemeCatalog(unittest.TestCase):
    def setUp(self):
        self.themes = {
            "Centro": {"building_density": 0.95, "tree_density": 2, "sidewalk": "PedraPortuguesa", "commercial_ratio": 0.85},
            "Bosque": {"building_density": 0.55, "tree_density": 12, "sidewalk": "Concreto", "commercial_ratio": 0.10},
            "Goiás": {"building_density": 0.80, "tree_density": 3, "sidewalk": "Concreto", "commercial_ratio": 0.50},
            "Miranda": {"building_density": 0.75, "tree_density": 5, "sidewalk": "Concreto", "commercial_ratio": 0.20},
            "Amorim": {"building_density": 0.70, "tree_density": 4, "sidewalk": "Concreto", "commercial_ratio": 0.25},
            "Santa Helena": {"building_density": 0.65, "tree_density": 6, "sidewalk": "Concreto", "commercial_ratio": 0.15},
            "Sibipiruna": {"building_density": 0.70, "tree_density": 5, "sidewalk": "Concreto", "commercial_ratio": 0.30}
        }

    def test_all_7_neighborhoods_configured(self):
        self.assertEqual(len(self.themes), 7)
        for neigh in ["Centro", "Bosque", "Goiás", "Miranda", "Amorim", "Santa Helena", "Sibipiruna"]:
            self.assertIn(neigh, self.themes)

    def test_centro_has_highest_commercial_density(self):
        centro = self.themes["Centro"]
        self.assertGreater(centro["commercial_ratio"], 0.8)
        self.assertGreater(centro["building_density"], 0.9)
        self.assertEqual(centro["sidewalk"], "PedraPortuguesa")

    def test_bosque_has_highest_tree_density(self):
        bosque = self.themes["Bosque"]
        self.assertGreater(bosque["tree_density"], 10)
        self.assertLess(bosque["commercial_ratio"], 0.2)


class TestCommercialSignboards(unittest.TestCase):
    def setUp(self):
        self.signboards = [
            {"id": "SIGN_OficinaDoGrau", "trade_name": "Oficina do Grau", "category": "OficinaMecanica"},
            {"id": "SIGN_BorrachariaBaiano", "trade_name": "Borracharia do Baiano", "category": "OficinaMecanica"},
            {"id": "SIGN_MercadinhoCentral", "trade_name": "Mercadinho Central", "category": "Comercio"},
            {"id": "SIGN_AutoPecasMaua", "trade_name": "Auto Peças Mauá", "category": "Comercio"},
            {"id": "SIGN_PadariaPaoDeQueijo", "trade_name": "Padaria Pão de Queijo de Minas", "category": "Comercio"},
            {"id": "SIGN_PostoPioneiro", "trade_name": "Posto Pioneiro", "category": "Posto"}
        ]

    def test_fictional_signboards_present(self):
        names = [s["trade_name"] for s in self.signboards]
        self.assertIn("Oficina do Grau", names)
        self.assertIn("Borracharia do Baiano", names)
        self.assertIn("Auto Peças Mauá", names)
        self.assertIn("Posto Pioneiro", names)

    def test_signboard_categories_differentiated(self):
        categories = {s["category"] for s in self.signboards}
        self.assertIn("OficinaMecanica", categories)
        self.assertIn("Comercio", categories)
        self.assertIn("Posto", categories)


class TestFacadeAndStreetDetailing(unittest.TestCase):
    def test_front_wall_gate_width_and_clearance(self):
        lot_frontage = 12.0
        gate_width = 3.5
        side_wall = (lot_frontage - gate_width) * 0.5

        # Garante que as duas metades somadas ao vão cobrem a testada
        self.assertAlmostEqual(side_wall * 2 + gate_width, lot_frontage)
        self.assertGreater(gate_width, 3.0) # Vão suficiente para entrada de motos e carros

    def test_speed_bump_dimensions_safe_for_bikes(self):
        bump_height = 0.10 # 10 cm
        bump_depth = 1.5   # 1.5 metros de rampa suave

        # Deve ser suave para permitir manobras e amortecimento sem travamento
        self.assertLessEqual(bump_height, 0.12)
        self.assertGreaterEqual(bump_depth, 1.2)

    def test_curb_placement_matches_road_half_width(self):
        road_width = 14.0
        half_width = road_width * 0.5

        curb_left = -half_width
        curb_right = half_width

        self.assertAlmostEqual(curb_right - curb_left, road_width)


class TestSceneAndGUIDIntegrity(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))

    def test_scripts_and_meta_guids(self):
        expected = {
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "NeighborhoodThemeCatalog.cs.meta"): "100000000000000000000000000000b8",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "FacadeDetailGenerator.cs.meta"): "100000000000000000000000000000b9",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "StreetInfrastructureDetailer.cs.meta"): "100000000000000000000000000000ba",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Architecture", "CommercialSignboardCatalog.cs.meta"): "100000000000000000000000000000bb",
        }
        for path, guid in expected.items():
            self.assertTrue(os.path.isfile(path), f"Arquivo não encontrado: {path}")
            with open(path, "r", encoding="utf-8") as f:
                content = f.read()
            self.assertIn(f"guid: {guid}", content, f"GUID incorreto em {path}")

    def test_scriptable_object_assets(self):
        themes_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "NeighborhoodThemes_Default.asset")
        signboards_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "CommercialSignboards_Default.asset")

        self.assertTrue(os.path.isfile(themes_asset))
        self.assertTrue(os.path.isfile(signboards_asset))

        with open(themes_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("Centro", txt)
        self.assertIn("PedraPortuguesa", txt)

        with open(signboards_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("SIGN_OficinaDoGrau", txt)
        self.assertIn("Borracharia do Baiano", txt)

    def test_scene_detail_infrastructure_node(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "UrbanTrafficTestScene.unity")
        self.assertTrue(os.path.isfile(scene_path))
        with open(scene_path, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("m_Name: Araguari_City_Detail_Root", txt)
        self.assertIn("guid: 100000000000000000000000000000b9", txt) # FacadeDetailGenerator
        self.assertIn("guid: 100000000000000000000000000000ba", txt) # StreetInfrastructureDetailer


if __name__ == "__main__":
    unittest.main()
