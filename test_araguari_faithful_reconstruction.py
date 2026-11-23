import unittest
import os
import json
import math
from shapely.geometry import Polygon, box
from shapely.ops import unary_union

class TestAraguariCoordinateSystem(unittest.TestCase):
    def setUp(self):
        self.origin_lat = -18.649100  # Praça Manoel Bonito, Araguari - MG
        self.origin_lon = -48.190300
        self.origin_alt = 970.0
        self.earth_radius = 6378137.0
        self.deg2rad = math.pi / 180.0
        self.cos_lat0 = math.cos(self.origin_lat * self.deg2rad)

    def geo_to_local(self, lat, lon):
        x = (lon - self.origin_lon) * self.deg2rad * self.earth_radius * self.cos_lat0
        z = (lat - self.origin_lat) * self.deg2rad * self.earth_radius
        return x, z

    def local_to_geo(self, x, z):
        lat = self.origin_lat + (z / (self.deg2rad * self.earth_radius))
        lon = self.origin_lon + (x / (self.deg2rad * self.earth_radius * self.cos_lat0))
        return lat, lon

    def test_origin_maps_to_zero(self):
        x, z = self.geo_to_local(self.origin_lat, self.origin_lon)
        self.assertAlmostEqual(x, 0.0, places=3)
        self.assertAlmostEqual(z, 0.0, places=3)

    def test_bidirectional_roundtrip_precision(self):
        test_lat = -18.647500
        test_lon = -48.188500

        x, z = self.geo_to_local(test_lat, test_lon)
        roundtrip_lat, roundtrip_lon = self.local_to_geo(x, z)

        self.assertAlmostEqual(test_lat, roundtrip_lat, places=6)
        self.assertAlmostEqual(test_lon, roundtrip_lon, places=6)

    def test_scale_metric_1_unit_equals_1_meter(self):
        delta_lat = 1.0 / (self.deg2rad * self.earth_radius)
        x, z = self.geo_to_local(self.origin_lat + delta_lat, self.origin_lon)

        self.assertAlmostEqual(z, 1.0, places=3)
        self.assertAlmostEqual(x, 0.0, places=3)


class TestIBGEPilotDatasetIntegrity(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))
        self.json_path = os.path.join(self.unity_root, "Assets", "StreamingAssets", "Geography", "araguari_pilot_geometry.json")
        self.assertTrue(os.path.isfile(self.json_path), f"Arquivo não encontrado: {self.json_path}")

        with open(self.json_path, "r", encoding="utf-8") as f:
            self.data = json.load(f)

    def test_metadata_completeness(self):
        meta = self.data.get("metadata", {})
        self.assertEqual(meta.get("city"), "Araguari")
        self.assertEqual(meta.get("state"), "MG")
        self.assertIn("IBGE Censo Demografico 2022", meta.get("source", ""))
        self.assertAlmostEqual(meta.get("crs_origin", {}).get("latitude"), -18.649100, places=5)
        self.assertAlmostEqual(meta.get("crs_origin", {}).get("longitude"), -48.190300, places=5)
        self.assertEqual(meta.get("crs_origin", {}).get("altitude_meters"), 970.0)

    def test_pilot_volume_and_coverage(self):
        quadras = self.data.get("quadras", [])
        corridors = self.data.get("corridors", [])
        faces = self.data.get("faces", [])
        intersections = self.data.get("intersections", [])

        self.assertEqual(len(quadras), 44, "A região piloto deve conter exatamente 44 quadras fechadas")
        self.assertGreaterEqual(len(corridors), 25, "A região piloto deve conter ao menos 25 corredores viários")
        self.assertEqual(len(faces), 179, "A região piloto contém 179 faces (148 no interior e 31 nas testadas de fechamento das quadras limítrofes)")
        self.assertGreaterEqual(len(intersections), 15, "A região piloto deve conter ao menos 15 cruzamentos conectados")

    def test_official_streets_present_in_pilot(self):
        streets = self.data.get("streets", [])
        required = [
            "AVENIDA TEODOLINO PEREIRA ARAUJO",
            "AVENIDA TIRADENTES",
            "PRACA MANOEL BONITO",
            "RUA MARCIANO SANTOS",
            "RUA RUI BARBOSA",
            "RUA BRASIL ACCIOLY",
            "RUA VIRGILIO DE MELO FRANCO"
        ]
        for req in required:
            self.assertTrue(
                any(req in s.upper() for s in streets),
                f"Logradouro obrigatório ausente do piloto: {req}"
            )

    def test_quadra_polygon_geometry_validation(self):
        quadras = self.data.get("quadras", [])
        for q in quadras:
            qid = q.get("quadra_id")
            vertices = q.get("polygon_vertices", [])
            area = q.get("polygon_area_unclipped_m2", 0)
            perimeter = q.get("perimeter_meters", 0)

            # 1. Pelo menos 3 vértices
            self.assertGreaterEqual(len(vertices), 3, f"Quadra com menos de 3 vértices: {qid}")

            # 2. Área não-degenerada (> 1000 m2)
            self.assertGreater(area, 1000.0, f"Área de quadra excessivamente reduzida ou degenerada: {qid}")

            # 3. Perímetro positivo e coerente (> 80m)
            self.assertGreater(perimeter, 80.0, f"Perímetro de quadra inconsistente: {qid}")

            # 4. Orientação CCW (área do shoelace positiva)
            n = len(vertices)
            signed_area = 0.0
            for i in range(n):
                j = (i + 1) % n
                signed_area += vertices[i]["x"] * vertices[j]["z"]
                signed_area -= vertices[j]["x"] * vertices[i]["z"]
            signed_area /= 2.0
            self.assertGreater(signed_area, 0.0, f"Orientação de vértices deve ser anti-horária (CCW): {qid}")

    def test_topological_area_consistency_and_zero_overlap(self):
        audit = self.data.get("metadata", {}).get("topological_audit", {})
        self.assertAlmostEqual(audit.get("pilot_box_area_m2"), 302500.0)
        self.assertEqual(audit.get("total_quadras"), 44)
        self.assertEqual(audit.get("quadras_entirely_inside"), 20)
        self.assertEqual(audit.get("quadras_intersecting_boundary"), 24)

        # A área de sobreposição entre quadras deve ser rigorosamente zero (< 0.1 m²)
        self.assertLess(audit.get("quadra_overlap_area_m2", 999), 0.1)

        # Cobertura urbana das quadras deve estar na faixa típica de 70% a 80%
        cov = audit.get("urban_block_coverage_pct", 0)
        self.assertGreaterEqual(cov, 70.0)
        self.assertLessEqual(cov, 80.0)

        # O restante é leito viário (ruas, avenidas, calçadas: 20% a 30%)
        street_area = audit.get("street_network_area_m2", 0)
        self.assertGreater(street_area, 60000.0)
        self.assertLess(street_area, 95000.0)

        # A soma das áreas recortadas deve bater exatamente com a área da união (disjunção estrita)
        sum_clipped = audit.get("sum_clipped_quadra_area_m2", 0)
        union_clipped = audit.get("union_clipped_quadra_area_m2", 0)
        self.assertAlmostEqual(sum_clipped, union_clipped, places=1)

    def test_separation_of_census_sectors_and_neighborhoods(self):
        quadras = self.data.get("quadras", [])
        for q in quadras:
            setor = q.get("setor_censitario", "")
            self.assertTrue(setor.startswith("3103504"), f"Código de setor censitário inválido: {setor}")
            self.assertEqual(q.get("neighborhood_layer"), "Centro")

    def test_road_corridor_widths_classification(self):
        corridors = self.data.get("corridors", [])
        measured_count = 0
        estimated_count = 0

        for c in corridors:
            self.assertIn("is_width_estimated", c)
            self.assertIn("confidence_level", c)
            width = c.get("road_width_meters", 0)
            self.assertGreaterEqual(width, 7.0, f"Largura de via improvável: {c['street_name']}")
            self.assertLessEqual(width, 35.0, f"Largura de via excessiva: {c['street_name']}")

            if c["is_width_estimated"]:
                estimated_count += 1
                self.assertEqual(c["confidence_level"], "ESTIMATIVA_CLASSIFICADA")
            else:
                measured_count += 1
                self.assertEqual(c["confidence_level"], "MEDICAO_FACES")

        self.assertGreater(measured_count, 0, "Deve haver vias com largura medida a partir de faces opostas")
        self.assertGreater(estimated_count, 0, "Deve haver vias com largura estimada na borda da bounding box")


class TestPilotSceneAssembly(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))
        self.scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "AraguariPilotCentroScene.unity")
        self.meta_path = self.scene_path + ".meta"

    def test_pilot_scene_files_exist(self):
        self.assertTrue(os.path.isfile(self.scene_path), f"Cena piloto ausente: {self.scene_path}")
        self.assertTrue(os.path.isfile(self.meta_path), f"Meta da cena piloto ausente: {self.meta_path}")

        with open(self.meta_path, "r", encoding="utf-8") as f:
            meta_txt = f.read()
        self.assertIn("guid: 30000000000000000000000000000006", meta_txt)

    def test_pilot_scene_gameobjects_and_components(self):
        with open(self.scene_path, "r", encoding="utf-8") as f:
            scene_txt = f.read()

        # 1. Superfície dirigível e Iluminação
        self.assertIn("m_Name: Araguari_Drivable_RoadSurface", scene_txt)
        self.assertIn("m_Name: Directional Light", scene_txt)

        # 2. Motocicleta Titan 160 completa e Piloto
        self.assertIn("m_Name: Motorcycle_Titan160", scene_txt)
        self.assertIn("m_Name: Rider_Humanoid", scene_txt)
        self.assertIn("guid: 10000000000000000000000000000005", scene_txt) # MotorcycleController
        self.assertIn("guid: 10000000000000000000000000000004", scene_txt) # WheelieController
        self.assertIn("guid: 10000000000000000000000000000014", scene_txt) # EngineController
        self.assertIn("guid: 10000000000000000000000000000015", scene_txt) # TransmissionController
        self.assertIn("guid: 10000000000000000000000000000016", scene_txt) # ECU
        self.assertIn("guid: 10000000000000000000000000000022", scene_txt) # Suspension
        self.assertIn("guid: 100000000000000000000000000000bd", scene_txt) # PlayerInteractionDetector

        # 3. Câmera com rastreamento
        self.assertIn("m_Name: Main Camera", scene_txt)
        self.assertIn("guid: 10000000000000000000000000000007", scene_txt) # CameraFollow

        # 4. Construtor Real e Infraestrutura Urbana
        self.assertIn("m_Name: Araguari_Real_City_Builder", scene_txt)
        self.assertIn("guid: 100000000000000000000000000000c4", scene_txt) # AraguariRealCityBuilder
        self.assertIn("m_Name: Araguari_City_Infrastructure", scene_txt)
        self.assertIn("m_Name: World_Infrastructure_Root", scene_txt)
        self.assertIn("guid: 100000000000000000000000000000a1", scene_txt) # WorldStreamer
        self.assertIn("guid: 100000000000000000000000000000a5", scene_txt) # WorldOptimizationManager

        # 5. Estabelecimentos Interativos
        self.assertIn("m_Name: Araguari_Interactive_Establishments", scene_txt)
        self.assertIn("guid: 100000000000000000000000000000c1", scene_txt) # InteriorManager
        self.assertIn("guid: 100000000000000000000000000000bf", scene_txt) # Oficina do Grau
        self.assertIn("guid: 100000000000000000000000000000c0", scene_txt) # Posto Pioneiro


class TestAndroidBuildAutomationScript(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))
        self.script_path = os.path.join(self.unity_root, "Assets", "Editor", "AndroidBuildAutomation.cs")
        self.meta_path = self.script_path + ".meta"
        self.version_path = os.path.join(self.unity_root, "ProjectSettings", "ProjectVersion.txt")

    def test_project_version_registered(self):
        self.assertTrue(os.path.isfile(self.version_path), f"ProjectVersion.txt ausente: {self.version_path}")
        with open(self.version_path, "r", encoding="utf-8") as f:
            v_txt = f.read()
        self.assertIn("6000.3.6f1", v_txt)

    def test_build_script_exists_and_configured(self):
        self.assertTrue(os.path.isfile(self.script_path))
        self.assertTrue(os.path.isfile(self.meta_path))

        with open(self.script_path, "r", encoding="utf-8") as f:
            content = f.read()

        self.assertIn("com.InsanityGroup.ProjetoGrau2", content)
        self.assertIn("AraguariPilotCentroScene.unity", content)
        self.assertIn("ARM64", content)
        self.assertIn("BuildDevApk", content)
        self.assertIn("using UnityEditor.Build;", content)

    def test_pilot_scene_transform_hierarchy_integrity(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "AraguariPilotCentroScene.unity")
        with open(scene_path, "r", encoding="utf-8") as f:
            scene_txt = f.read()

        # Verifica se as ancoras de roda possuem GameObjects próprios
        self.assertIn("m_Name: FrontWheelAnchor", scene_txt)
        self.assertIn("m_Name: RearWheelAnchor", scene_txt)
        self.assertIn("m_GameObject: {fileID: 312}", scene_txt)
        self.assertIn("m_GameObject: {fileID: 313}", scene_txt)


if __name__ == "__main__":
    unittest.main()
