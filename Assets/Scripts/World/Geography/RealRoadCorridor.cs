using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.World.Geography
{
    /// <summary>
    /// Corredor viário real derivado da geometria de faces de quadras do IBGE em Araguari.
    /// Define o eixo viário (centerline), a largura do leito carroçável e as calçadas,
    /// provendo suporte para navegação de veículos civis e policiais.
    /// </summary>
    [AddComponentMenu("Rery City/Geography/Real Road Corridor")]
    public class RealRoadCorridor : MonoBehaviour
    {
        [Header("Identificação do Logradouro")]
        [SerializeField] private string streetName = "Rua Rui Barbosa";
        [SerializeField] private string neighborhood = "Centro";

        [Header("Perfil Transversal e Circulação")]
        [SerializeField] private float roadWidthMeters = 10.0f;
        [SerializeField] private float sidewalkWidthMeters = 2.5f;
        [SerializeField] private int laneCount = 2;
        [SerializeField] private bool isOneWay = false;
        [SerializeField] private float speedLimitKmh = 40.0f;

        [Header("Eixo Viário (Centerline)")]
        [SerializeField] private List<Vector3> centerlineNodes = new List<Vector3>();
        [SerializeField] private List<TrafficWaypoint> generatedWaypoints = new List<TrafficWaypoint>();

        public string StreetName => streetName;
        public string Neighborhood => neighborhood;
        public float RoadWidthMeters => roadWidthMeters;
        public float SidewalkWidthMeters => sidewalkWidthMeters;
        public int LaneCount => laneCount;
        public bool IsOneWay => isOneWay;
        public float SpeedLimitKmh => speedLimitKmh;
        public IReadOnlyList<Vector3> CenterlineNodes => centerlineNodes;
        public IReadOnlyList<TrafficWaypoint> GeneratedWaypoints => generatedWaypoints;

        public void InitializeCorridor(string name, string neigh, float width, int lanes, bool oneWay, float speed)
        {
            streetName = name;
            neighborhood = neigh;
            roadWidthMeters = width;
            laneCount = lanes;
            isOneWay = oneWay;
            speedLimitKmh = speed;
        }

        public void SetCenterline(IEnumerable<Vector3> nodes)
        {
            centerlineNodes.Clear();
            if (nodes != null)
            {
                centerlineNodes.AddRange(nodes);
            }
        }

        public void AddCenterlineNode(Vector3 node)
        {
            centerlineNodes.Add(node);
        }

        public float CalculateTotalLength()
        {
            if (centerlineNodes.Count < 2) return 0f;
            float len = 0f;
            for (int i = 0; i < centerlineNodes.Count - 1; i++)
            {
                len += Vector3.Distance(centerlineNodes[i], centerlineNodes[i + 1]);
            }
            return len;
        }

        public Vector3 GetClosestPointOnCenterline(Vector3 queryPos, out float distanceSqr)
        {
            distanceSqr = float.MaxValue;
            if (centerlineNodes.Count == 0) return queryPos;
            if (centerlineNodes.Count == 1)
            {
                distanceSqr = (centerlineNodes[0] - queryPos).sqrMagnitude;
                return centerlineNodes[0];
            }

            Vector3 bestPt = centerlineNodes[0];
            for (int i = 0; i < centerlineNodes.Count - 1; i++)
            {
                Vector3 a = centerlineNodes[i];
                Vector3 b = centerlineNodes[i + 1];
                Vector3 ab = b - a;
                float abSqr = ab.sqrMagnitude;
                if (abSqr < 0.0001f) continue;

                float t = Mathf.Clamp01(Vector3.Dot(queryPos - a, ab) / abSqr);
                Vector3 proj = a + ab * t;
                float dSqr = (proj - queryPos).sqrMagnitude;
                if (dSqr < distanceSqr)
                {
                    distanceSqr = dSqr;
                    bestPt = proj;
                }
            }

            return bestPt;
        }
    }
}
