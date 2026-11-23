using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Geography
{
    [Serializable]
    public class Vector2Local
    {
        public float x;
        public float z;

        public Vector3 ToVector3(float y = 0f) => new Vector3(x, y, z);
    }

    [Serializable]
    public class Vector2Geo
    {
        public double lon;
        public double lat;
    }

    [Serializable]
    public class CrsOriginData
    {
        public string name;
        public double latitude;
        public double longitude;
        public double altitude_meters;
    }

    [Serializable]
    public class PilotBoundsData
    {
        public float half_extent_meters;
        public float total_area_meters2;
    }

    [Serializable]
    public class CityGeoMetadata
    {
        public string city;
        public string state;
        public string source;
        public string license;
        public CrsOriginData crs_origin;
        public PilotBoundsData pilot_bounds;
        public int total_faces;
        public int total_quadras;
        public int total_streets;
        public int total_intersections;
    }

    [Serializable]
    public class QuadraBoundsData
    {
        public float min_x;
        public float max_x;
        public float min_z;
        public float max_z;
        public float width_meters;
        public float length_meters;
    }

    [Serializable]
    public class RealQuadraData
    {
        public string quadra_id;
        public string setor_censitario;
        public string quadra_num;
        public string neighborhood_layer;
        public int face_count;
        public Vector2Local center;
        public List<Vector2Local> polygon_vertices;
        public float polygon_area_m2;
        public float perimeter_meters;
        public QuadraBoundsData bounds;
        public int total_res;
        public int total_geral;
        public List<string> flanking_streets;
    }

    [Serializable]
    public class RealFaceData
    {
        public string face_id;
        public string setor_censitario;
        public string quadra_num;
        public string face_num;
        public string street_name;
        public int tot_res;
        public int tot_geral;
        public List<Vector2Local> points_local;
        public List<Vector2Geo> points_geo;
    }

    [Serializable]
    public class RoadCorridorData
    {
        public string street_name;
        public string neighborhood;
        public bool is_east_west;
        public float road_width_meters;
        public bool is_width_estimated;
        public string confidence_level;
        public float sidewalk_width_meters;
        public int lane_count;
        public float speed_limit_kmh;
        public List<Vector2Local> centerline_nodes;
        public int face_count;
    }

    [Serializable]
    public class IntersectionData
    {
        public string intersection_id;
        public string street_a;
        public string street_b;
        public Vector2Local position;
    }

    /// <summary>
    /// Contêiner de dados geoespaciais reais de Araguari (MG) desserializado do GeoJSON processado do IBGE Censo 2022.
    /// </summary>
    [Serializable]
    public class RealCityGeometryPayload
    {
        public CityGeoMetadata metadata;
        public List<RealQuadraData> quadras;
        public List<RoadCorridorData> corridors;
        public List<IntersectionData> intersections;
        public List<string> streets;
        public List<RealFaceData> faces;
    }
}
