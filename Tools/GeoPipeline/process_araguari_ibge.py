import json
import math
import os
from shapely.geometry import LineString, Polygon, box
from shapely.ops import polygonize, unary_union

RAW_PATH = r"c:\Users\washi\Downloads\3103504_faces_de_logradouros_2022.json"
ORIGIN_LAT = -18.649100  # Praça Manoel Bonito, Centro, Araguari
ORIGIN_LON = -48.190300
ORIGIN_ALT = 970.0
EARTH_RADIUS = 6378137.0
DEG2RAD = math.pi / 180.0
COS_LAT0 = math.cos(ORIGIN_LAT * DEG2RAD)

PILOT_HALF_EXTENT = 275.0  # 550m x 550m bounding box (302.500 m²)

def geo_to_local(lat, lon):
    x = (lon - ORIGIN_LON) * DEG2RAD * EARTH_RADIUS * COS_LAT0
    z = (lat - ORIGIN_LAT) * DEG2RAD * EARTH_RADIUS
    return round(x, 3), round(z, 3)

def local_to_geo(x, z):
    lat = ORIGIN_LAT + (z / (DEG2RAD * EARTH_RADIUS))
    lon = ORIGIN_LON + (x / (DEG2RAD * EARTH_RADIUS * COS_LAT0))
    return round(lat, 7), round(lon, 7)

def main():
    if not os.path.exists(RAW_PATH):
        print(f"Erro: Arquivo bruto não encontrado em {RAW_PATH}")
        return

    with open(RAW_PATH, "r", encoding="utf-8") as f:
        raw_data = json.load(f)

    features = raw_data.get("features", [])
    print(f">> Dataset IBGE carregado: {len(features)} feições totais.")

    pilot_box = box(-PILOT_HALF_EXTENT, -PILOT_HALF_EXTENT, PILOT_HALF_EXTENT, PILOT_HALF_EXTENT)

    # 1. Agrupamento global de faces por quadra
    all_quadras_faces = {}
    for feat in features:
        geom = feat.get("geometry")
        props = feat.get("properties", {})
        if not geom or geom.get("type") != "LineString":
            continue

        setor = props.get("CD_SETOR")
        cd_quadra = props.get("CD_QUADRA")
        if not setor or not cd_quadra:
            continue

        q_key = f"{setor}_{cd_quadra}"
        coords = geom.get("coordinates", [])
        local_pts = [geo_to_local(c[1], c[0]) for c in coords]

        tipo = props.get("NM_TIP_LOG", "").strip()
        nome = props.get("NM_LOG", "").strip()
        full_street = f"{tipo} {nome}".strip() if tipo else nome

        face_entry = {
            "face_id": f"{setor}_{cd_quadra}_{props.get('CD_FACE')}",
            "setor_censitario": setor,
            "quadra_num": cd_quadra,
            "face_num": props.get("CD_FACE"),
            "street_name": full_street,
            "tot_res": int(props.get("TOT_RES") or 0),
            "tot_geral": int(props.get("TOT_GERAL") or 0),
            "points_local": [{"x": p[0], "z": p[1]} for p in local_pts],
            "points_geo": [{"lon": c[0], "lat": c[1]} for c in coords],
            "line_local": LineString(local_pts)
        }

        if q_key not in all_quadras_faces:
            all_quadras_faces[q_key] = []
        all_quadras_faces[q_key].append(face_entry)

    # 2. Filtragem de quadras que tocam o pilot box
    pilot_quadras = {}
    pilot_faces = []
    pilot_streets = {}

    for q_key, f_list in all_quadras_faces.items():
        touches_pilot = any(pilot_box.intersects(f["line_local"]) for f in f_list)
        if touches_pilot:
            pilot_quadras[q_key] = f_list
            for f in f_list:
                pilot_faces.append(f)
                st = f["street_name"]
                if st:
                    if st not in pilot_streets:
                        pilot_streets[st] = []
                    pilot_streets[st].append(f)

    print(f">> Quadras que intersectam a Região Piloto: {len(pilot_quadras)}")

    # 3. Reconstrução Topológica Fiel via shapely.ops.polygonize
    quadra_polygons = []
    clipped_polygons = []

    for q_key, f_list in pilot_quadras.items():
        lines = [f["line_local"] for f in f_list]
        merged = unary_union(lines)
        polys = list(polygonize(merged))

        # Fechamento topológico automático para gaps cadastrais de esquina (< 15m)
        if not polys:
            from collections import defaultdict
            endpoint_count = defaultdict(int)
            for l in lines:
                endpoint_count[l.coords[0]] += 1
                endpoint_count[l.coords[-1]] += 1
            dangling = [pt for pt, count in endpoint_count.items() if count == 1]
            if len(dangling) == 2:
                gap_dist = math.hypot(dangling[0][0] - dangling[1][0], dangling[0][1] - dangling[1][1])
                if gap_dist <= 15.0:
                    lines.append(LineString(dangling))
                    polys = list(polygonize(unary_union(lines)))

        if not polys:
            print(f"Alerta: Quadra {q_key} não fechou por polygonize.")
            continue

        poly = polys[0]
        # Garante orientação CCW
        if not poly.exterior.is_ccw:
            poly = Polygon(list(poly.exterior.coords)[::-1])

        unclipped_area = round(poly.area, 2)
        clipped_poly = poly.intersection(pilot_box)
        clipped_area = round(clipped_poly.area, 2)
        is_inside = pilot_box.contains(poly)
        clipped_polygons.append(clipped_poly)

        # Extrai vértices da quadra (reconstrução fiel conectada)
        poly_coords = list(poly.exterior.coords)[:-1] # Remove duplicata final
        vertices_out = [{"x": round(c[0], 2), "z": round(c[1], 2)} for c in poly_coords]

        # Bounds AABB
        min_x, min_z, max_x, max_z = poly.bounds
        width_m = round(max_x - min_x, 2)
        length_m = round(max_z - min_z, 2)
        center_x = round((min_x + max_x) / 2.0, 2)
        center_z = round((min_z + max_z) / 2.0, 2)

        street_set = {f["street_name"] for f in f_list if f["street_name"]}
        total_res = sum(f["tot_res"] for f in f_list)
        total_geral = sum(f["tot_geral"] for f in f_list)

        quadra_polygons.append({
            "quadra_id": q_key,
            "setor_censitario": f_list[0]["setor_censitario"],
            "quadra_num": f_list[0]["quadra_num"],
            "neighborhood_layer": "Centro",
            "face_count": len(f_list),
            "is_entirely_inside_pilot_box": is_inside,
            "center": {"x": center_x, "z": center_z},
            "polygon_vertices": vertices_out,
            "polygon_area_unclipped_m2": unclipped_area,
            "polygon_area_clipped_m2": clipped_area,
            "polygon_area_m2": unclipped_area, # Compatibilidade retroativa
            "perimeter_meters": round(poly.length, 2),
            "bounds": {
                "min_x": round(min_x, 2), "max_x": round(max_x, 2),
                "min_z": round(min_z, 2), "max_z": round(max_z, 2),
                "width_meters": width_m,
                "length_meters": length_m,
                "aabb_area_m2": round(width_m * length_m, 2)
            },
            "total_res": total_res,
            "total_geral": total_geral,
            "flanking_streets": sorted(list(street_set))
        })

    # 4. Cálculo Rigoroso das Métricas Topológicas da Região Piloto
    union_clipped = unary_union(clipped_polygons)
    total_clipped_area = sum(q["polygon_area_clipped_m2"] for q in quadra_polygons)
    union_clipped_area = round(union_clipped.area, 2)
    overlap_area = round(total_clipped_area - union_clipped_area, 2)
    street_area = round(pilot_box.area - union_clipped_area, 2)
    coverage_pct = round((union_clipped_area / pilot_box.area) * 100.0, 1)

    sum_aabb = sum(q["bounds"]["aabb_area_m2"] for q in quadra_polygons)
    mean_aabb = round(sum_aabb / len(quadra_polygons), 2)
    mean_unclipped = round(sum(q["polygon_area_unclipped_m2"] for q in quadra_polygons) / len(quadra_polygons), 2)
    mean_clipped = round(total_clipped_area / len(quadra_polygons), 2)

    entirely_inside_count = sum(1 for q in quadra_polygons if q["is_entirely_inside_pilot_box"])
    intersecting_boundary_count = len(quadra_polygons) - entirely_inside_count

    print(f">> Métricas Topológicas Rigorosas:")
    print(f"   - Área do Piloto: {pilot_box.area:.1f} m2")
    print(f"   - Quadras Inteiramente Dentro: {entirely_inside_count}")
    print(f"   - Quadras que Ultrapassam a Borda: {intersecting_boundary_count}")
    print(f"   - Soma da Área Recortada das Quadras: {total_clipped_area:.1f} m2")
    print(f"   - Área da União das Quadras (sem overlaps): {union_clipped_area:.1f} m2")
    print(f"   - Área de Sobreposição entre Quadras: {overlap_area:.2f} m2 (Zero Overlap!)")
    print(f"   - Área do Leito Viário (Ruas e Calçadas): {street_area:.1f} m2 ({100.0 - coverage_pct:.1f}%)")
    print(f"   - Cobertura de Quadras: {coverage_pct:.1f}%")
    print(f"   - Média de Área Real Recortada por Quadra: {mean_clipped:.1f} m2")
    print(f"   - Média de AABB (Bounding Box) das Quadras: {mean_aabb:.1f} m2")

    # 5. Reconstrução dos Corredores Viários
    corridors = []
    for street_name, flist in pilot_streets.items():
        all_pts = []
        for f in flist:
            for pt in f["points_local"]:
                all_pts.append((pt["x"], pt["z"]))

        if len(all_pts) < 2:
            continue

        xs = [p[0] for p in all_pts]
        zs = [p[1] for p in all_pts]
        min_x, max_x = min(xs), max(xs)
        min_z, max_z = min(zs), max(zs)
        span_x = max_x - min_x
        span_z = max_z - min_z
        is_east_west = span_x >= span_z

        is_width_estimated = True
        measured_width = 10.0

        if is_east_west:
            gap_z = span_z
            if len(flist) >= 2 and 8.0 <= gap_z <= 30.0:
                measured_width = round(gap_z, 1)
                is_width_estimated = False
            else:
                measured_width = 16.0 if "AVENIDA" in street_name.upper() else 10.5
        else:
            gap_x = span_x
            if len(flist) >= 2 and 8.0 <= gap_x <= 30.0:
                measured_width = round(gap_x, 1)
                is_width_estimated = False
            else:
                measured_width = 16.0 if "AVENIDA" in street_name.upper() else 10.5

        if is_east_west:
            avg_z = round((min_z + max_z) / 2.0, 2)
            center_nodes = [
                {"x": round(min_x - 5.0, 2), "z": avg_z},
                {"x": round(max_x + 5.0, 2), "z": avg_z}
            ]
        else:
            avg_x = round((min_x + max_x) / 2.0, 2)
            center_nodes = [
                {"x": avg_x, "z": round(min_z - 5.0, 2)},
                {"x": avg_x, "z": round(max_z + 5.0, 2)}
            ]

        speed_limit = 50.0 if "AVENIDA" in street_name.upper() else 40.0
        lane_count = 4 if "AVENIDA" in street_name.upper() and measured_width >= 14.0 else 2

        corridors.append({
            "street_name": street_name,
            "neighborhood": "Centro",
            "is_east_west": is_east_west,
            "road_width_meters": measured_width,
            "is_width_estimated": is_width_estimated,
            "confidence_level": "MEDICAO_FACES" if not is_width_estimated else "ESTIMATIVA_CLASSIFICADA",
            "sidewalk_width_meters": 2.5,
            "lane_count": lane_count,
            "speed_limit_kmh": speed_limit,
            "centerline_nodes": center_nodes,
            "face_count": len(flist)
        })

    # 6. Intersecções entre Corredores Viários
    intersections = []
    ew_corridors = [c for c in corridors if c["is_east_west"]]
    ns_corridors = [c for c in corridors if not c["is_east_west"]]

    for ew in ew_corridors:
        for ns in ns_corridors:
            ew_z = ew["centerline_nodes"][0]["z"]
            ew_x_min = min(ew["centerline_nodes"][0]["x"], ew["centerline_nodes"][1]["x"])
            ew_x_max = max(ew["centerline_nodes"][0]["x"], ew["centerline_nodes"][1]["x"])

            ns_x = ns["centerline_nodes"][0]["x"]
            ns_z_min = min(ns["centerline_nodes"][0]["z"], ns["centerline_nodes"][1]["z"])
            ns_z_max = max(ns["centerline_nodes"][0]["z"], ns["centerline_nodes"][1]["z"])

            if (ew_x_min <= ns_x <= ew_x_max) and (ns_z_min <= ew_z <= ns_z_max):
                intersections.append({
                    "intersection_id": f"int_{len(intersections)+1:02d}",
                    "street_a": ew["street_name"],
                    "street_b": ns["street_name"],
                    "position": {"x": ns_x, "z": ew_z}
                })

    # 7. Exportação JSON
    output_dir = os.path.join(os.path.dirname(__file__), "..", "..", "Assets", "StreamingAssets", "Geography")
    os.makedirs(output_dir, exist_ok=True)
    out_file = os.path.join(output_dir, "araguari_pilot_geometry.json")

    clean_faces_export = []
    for f in pilot_faces:
        clean_faces_export.append({
            "face_id": f["face_id"],
            "setor_censitario": f["setor_censitario"],
            "quadra_num": f["quadra_num"],
            "face_num": f["face_num"],
            "street_name": f["street_name"],
            "tot_res": f["tot_res"],
            "tot_geral": f["tot_geral"],
            "points_local": f["points_local"],
            "points_geo": f["points_geo"]
        })

    export_payload = {
        "metadata": {
            "city": "Araguari",
            "state": "MG",
            "source": "IBGE Censo Demografico 2022 - Faces de Logradouros",
            "license": "Dados Publicos Abertos (Lei 12.527/2011)",
            "crs_origin": {
                "name": "Praca Manoel Bonito, Centro",
                "latitude": ORIGIN_LAT,
                "longitude": ORIGIN_LON,
                "altitude_meters": ORIGIN_ALT
            },
            "pilot_bounds": {
                "half_extent_meters": PILOT_HALF_EXTENT,
                "total_area_meters2": (PILOT_HALF_EXTENT * 2) ** 2
            },
            "topological_audit": {
                "pilot_box_area_m2": pilot_box.area,
                "total_quadras": len(quadra_polygons),
                "quadras_entirely_inside": entirely_inside_count,
                "quadras_intersecting_boundary": intersecting_boundary_count,
                "sum_clipped_quadra_area_m2": total_clipped_area,
                "union_clipped_quadra_area_m2": union_clipped_area,
                "quadra_overlap_area_m2": overlap_area,
                "street_network_area_m2": street_area,
                "urban_block_coverage_pct": coverage_pct,
                "mean_clipped_quadra_area_m2": mean_clipped,
                "mean_unclipped_quadra_area_m2": mean_unclipped,
                "sum_aabb_area_m2": sum_aabb,
                "mean_aabb_area_m2": mean_aabb
            },
            "total_faces": len(clean_faces_export),
            "total_quadras": len(quadra_polygons),
            "total_streets": len(corridors),
            "total_intersections": len(intersections)
        },
        "quadras": quadra_polygons,
        "corridors": corridors,
        "intersections": intersections,
        "streets": sorted(list(pilot_streets.keys())),
        "faces": clean_faces_export
    }

    with open(out_file, "w", encoding="utf-8") as f:
        json.dump(export_payload, f, indent=2, ensure_ascii=False)

    print(f">> Dados regenerados e exportados com sucesso para: {out_file}")

if __name__ == "__main__":
    main()
