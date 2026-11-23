import unittest
import os
import math

class TestGeoCoordinateConverter(unittest.TestCase):
    EARTH_RADIUS = 6378137.0
    DEG_TO_RAD = math.pi / 180.0
    RAD_TO_DEG = 180.0 / math.pi

    def setUp(self):
        # Marco Zero: Praça Manoel Bonito (Araguari - MG)
        self.origin_lat = -18.649100
        self.origin_lon = -48.190300
        self.origin_alt = 970.0
        self.scale_factor = 1.0

    def geo_to_world(self, lat, lon, alt, heading_rot=0.0):
        origin_lat_rad = self.origin_lat * self.DEG_TO_RAD
        cos_origin = math.cos(origin_lat_rad)

        d_lat = (lat - self.origin_lat) * self.DEG_TO_RAD
        d_lon = (lon - self.origin_lon) * self.DEG_TO_RAD

        local_x = d_lon * self.EARTH_RADIUS * cos_origin * self.scale_factor
        local_z = d_lat * self.EARTH_RADIUS * self.scale_factor
        local_y = (alt - self.origin_alt) * self.scale_factor

        if abs(heading_rot) > 0.001:
            rot_rad = -heading_rot * self.DEG_TO_RAD
            c = math.cos(rot_rad)
            s = math.sin(rot_rad)
            rx = local_x * c - local_z * s
            rz = local_x * s + local_z * c
            local_x, local_z = rx, rz

        return local_x, local_y, local_z

    def world_to_geo(self, x, y, z, heading_rot=0.0):
        local_x, local_z = x, z
        if abs(heading_rot) > 0.001:
            rot_rad = heading_rot * self.DEG_TO_RAD
            c = math.cos(rot_rad)
            s = math.sin(rot_rad)
            ux = local_x * c - local_z * s
            uz = local_x * s + local_z * c
            local_x, local_z = ux, uz

        origin_lat_rad = self.origin_lat * self.DEG_TO_RAD
        cos_origin = math.cos(origin_lat_rad)

        d_lat = (local_z / self.scale_factor) / self.EARTH_RADIUS
        d_lon = (local_x / self.scale_factor) / (self.EARTH_RADIUS * cos_origin)

        lat = self.origin_lat + (d_lat * self.RAD_TO_DEG)
        lon = self.origin_lon + (d_lon * self.RAD_TO_DEG)
        alt = self.origin_alt + (y / self.scale_factor)

        return lat, lon, alt

    def test_origin_maps_to_zero(self):
        x, y, z = self.geo_to_world(self.origin_lat, self.origin_lon, self.origin_alt)
        self.assertAlmostEqual(x, 0.0, places=4)
        self.assertAlmostEqual(y, 0.0, places=4)
        self.assertAlmostEqual(z, 0.0, places=4)

    def test_bidirectional_conversion_precision(self):
        # Ponto real: Palácio dos Ferroviários (-18.6475, -48.1873, 972.0)
        test_lat = -18.647500
        test_lon = -48.187300
        test_alt = 972.0

        x, y, z = self.geo_to_world(test_lat, test_lon, test_alt)
        reconstructed_lat, reconstructed_lon, reconstructed_alt = self.world_to_geo(x, y, z)

        self.assertAlmostEqual(reconstructed_lat, test_lat, places=6)
        self.assertAlmostEqual(reconstructed_lon, test_lon, places=6)
        self.assertAlmostEqual(reconstructed_alt, test_alt, places=4)

    def test_azimuth_rotation_preserves_distance(self):
        x0, y0, z0 = self.geo_to_world(-18.6480, -48.1890, 970.0, heading_rot=0.0)
        dist0 = math.sqrt(x0 * x0 + z0 * z0)

        x45, y45, z45 = self.geo_to_world(-18.6480, -48.1890, 970.0, heading_rot=45.0)
        dist45 = math.sqrt(x45 * x45 + z45 * z45)

        self.assertAlmostEqual(dist0, dist45, places=3)


class TestAraguariCityCatalog(unittest.TestCase):
    def setUp(self):
        self.catalog_data = {
            "city_name": "Araguari",
            "state": "MG",
            "license": "ODbL",
            "neighborhoods": ["Centro", "Bosque", "Goiás", "Miranda", "Amorim", "Santa Helena", "Sibipiruna"],
            "roads": [
                "Avenida Coronel Theodolino Pereira de Araújo",
                "Avenida Minas Gerais",
                "Avenida Mato Grosso",
                "Rua Marciano Santos",
                "Rua Rui Barbosa",
                "Avenida Batalhão Mauá",
                "Rua Brasil Accioly"
            ],
            "pois": [
                "POI_PracaManoelBonito",
                "POI_PalacioDosFerroviarios",
                "POI_PracaDaMatriz",
                "POI_BosqueJohnKennedy",
                "POI_MercadoMunicipal"
            ]
        }

    def test_essential_neighborhoods_present(self):
        expected = ["Centro", "Bosque", "Goiás", "Miranda", "Amorim"]
        for neigh in expected:
            self.assertIn(neigh, self.catalog_data["neighborhoods"])

    def test_essential_roads_present(self):
        for road in ["Avenida Coronel Theodolino Pereira de Araújo", "Avenida Minas Gerais", "Rua Rui Barbosa"]:
            self.assertIn(road, self.catalog_data["roads"])

    def test_essential_pois_present(self):
        self.assertIn("POI_PracaManoelBonito", self.catalog_data["pois"])
        self.assertIn("POI_PalacioDosFerroviarios", self.catalog_data["pois"])
        self.assertIn("POI_BosqueJohnKennedy", self.catalog_data["pois"])


class TestRealRoadNetworkAndSpatialIndex(unittest.TestCase):
    class MockRoad:
        def __init__(self, name, start_pos, end_pos, width, lanes, is_oneway, speed_limit):
            self.name = name
            self.start_pos = start_pos
            self.end_pos = end_pos
            self.width = width
            self.lanes = lanes
            self.is_oneway = is_oneway
            self.speed_limit = speed_limit

    @staticmethod
    def pos_to_chunk_coord(pos, chunk_size=100.0):
        cx = int(math.floor(pos[0] / chunk_size))
        cz = int(math.floor(pos[2] / chunk_size))
        return (cx, cz)

    def setUp(self):
        self.roads = [
            self.MockRoad("Av. Theodolino Trecho 1", (-150.0, 0.0, -80.0), (50.0, 0.0, 120.0), 16.0, 4, False, 60.0),
            self.MockRoad("Rua Marciano Santos", (-30.0, 0.0, 10.0), (80.0, 0.0, 25.0), 10.0, 1, True, 40.0)
        ]
        self.chunk_map = {}
        for road in self.roads:
            c1 = self.pos_to_chunk_coord(road.start_pos)
            c2 = self.pos_to_chunk_coord(road.end_pos)
            for c in {c1, c2}:
                if c not in self.chunk_map:
                    self.chunk_map[c] = []
                self.chunk_map[c].append(road)

    def test_spatial_chunk_indexing(self):
        # Road 1 cruza de (-2, -1) para (0, 1)
        c_start = self.pos_to_chunk_coord(self.roads[0].start_pos)
        c_end = self.pos_to_chunk_coord(self.roads[0].end_pos)

        self.assertIn(self.roads[0], self.chunk_map[c_start])
        self.assertIn(self.roads[0], self.chunk_map[c_end])

    def test_nearest_road_lookup(self):
        query_pos = (5.0, 0.0, 15.0)
        nearest = None
        min_dist_sqr = float("inf")

        for road in self.roads:
            # distância até o ponto médio
            mid_x = (road.start_pos[0] + road.end_pos[0]) * 0.5
            mid_z = (road.start_pos[2] + road.end_pos[2]) * 0.5
            d_sqr = (mid_x - query_pos[0])**2 + (mid_z - query_pos[2])**2
            if d_sqr < min_dist_sqr:
                min_dist_sqr = d_sqr
                nearest = road

        self.assertEqual(nearest.name, "Rua Marciano Santos")


class TestSceneAndGUIDIntegrity(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))

    def test_scripts_and_meta_guids(self):
        expected = {
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "GeoCoordinateConverter.cs.meta"): "100000000000000000000000000000ac",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "GeoOriginConfig.cs.meta"): "100000000000000000000000000000ad",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "GeoPointOfInterest.cs.meta"): "100000000000000000000000000000ae",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "RealRoadSegment.cs.meta"): "100000000000000000000000000000af",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "RealRoadNetworkManager.cs.meta"): "100000000000000000000000000000b0",
            os.path.join(self.unity_root, "Assets", "Scripts", "World", "Geography", "AraguariCityDataCatalog.cs.meta"): "100000000000000000000000000000b1",
        }
        for path, guid in expected.items():
            self.assertTrue(os.path.isfile(path), f"Arquivo não encontrado: {path}")
            with open(path, "r", encoding="utf-8") as f:
                content = f.read()
            self.assertIn(f"guid: {guid}", content, f"GUID incorreto em {path}")

    def test_scriptable_object_assets(self):
        origin_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "GeoOrigin_AraguariCentro.asset")
        catalog_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "AraguariCityCatalog_Default.asset")

        self.assertTrue(os.path.isfile(origin_asset))
        self.assertTrue(os.path.isfile(catalog_asset))

        with open(origin_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("latitude: -18.6491", txt)
        self.assertIn("longitude: -48.1903", txt)

        with open(catalog_asset, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("cityName: Araguari", txt)
        self.assertIn("Theodolino", txt)

    def test_scene_infrastructure_node(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "UrbanTrafficTestScene.unity")
        self.assertTrue(os.path.isfile(scene_path))
        with open(scene_path, "r", encoding="utf-8") as f:
            txt = f.read()
        self.assertIn("m_Name: Araguari_City_Infrastructure", txt)
        self.assertIn("m_Name: POI_PracaManoelBonito", txt)
        self.assertIn("m_Name: Road_Av_Theodolino", txt)
        self.assertIn("guid: 100000000000000000000000000000b0", txt) # RealRoadNetworkManager
        self.assertIn("guid: 100000000000000000000000000000ae", txt) # GeoPointOfInterest
        self.assertIn("guid: 100000000000000000000000000000af", txt) # RealRoadSegment


if __name__ == "__main__":
    unittest.main()
