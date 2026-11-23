using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Traffic;
using ProjetoGrau2.World;

namespace ProjetoGrau2.World.Geography
{
    /// <summary>
    /// Gerenciador central da malha viária real de Araguari em RERY CITY.
    /// Indexa segmentos viários, rotatórias e conexões entre bairros, integrando-se
    /// diretamente com o TrafficNavigationManager e o WorldStreamer.
    /// </summary>
    [AddComponentMenu("Rery City/Geography/Real Road Network Manager")]
    public class RealRoadNetworkManager : MonoBehaviour
    {
        public static RealRoadNetworkManager Instance { get; private set; }

        [Header("Configuração Cartográfica")]
        [SerializeField] private GeoOriginConfig geoOrigin;
        [SerializeField] private AraguariCityDataCatalog cityCatalog;

        [Header("Segmentos e Vias Mapeadas")]
        [SerializeField] private List<RealRoadSegment> roadSegments = new List<RealRoadSegment>();
        [SerializeField] private List<GeoPointOfInterest> pointsOfInterest = new List<GeoPointOfInterest>();

        // Indexação espacial por Chunk para World Streaming
        private readonly Dictionary<WorldChunkCoord, List<RealRoadSegment>> _chunkRoadMap = new Dictionary<WorldChunkCoord, List<RealRoadSegment>>();

        public GeoOriginConfig GeoOrigin => geoOrigin;
        public AraguariCityDataCatalog CityCatalog => cityCatalog;
        public IReadOnlyList<RealRoadSegment> RoadSegments => roadSegments;
        public IReadOnlyList<GeoPointOfInterest> PointsOfInterest => pointsOfInterest;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BuildSpatialIndex();
        }

        /// <summary>
        /// Constrói o índice espacial de vias baseado nas coordenadas dos chunks do WorldStreamer.
        /// </summary>
        public void BuildSpatialIndex(float chunkSize = 100f)
        {
            _chunkRoadMap.Clear();

            for (int i = 0; i < roadSegments.Count; i++)
            {
                var road = roadSegments[i];
                if (road == null) continue;

                Vector3 startPos = GeoCoordinateConverter.GeoToWorld(road.StartLat, road.StartLon, 970.0, geoOrigin);
                Vector3 endPos = GeoCoordinateConverter.GeoToWorld(road.EndLat, road.EndLon, 970.0, geoOrigin);

                WorldChunkCoord startCoord = WorldChunkCoord.FromWorldPosition(startPos, chunkSize);
                WorldChunkCoord endCoord = WorldChunkCoord.FromWorldPosition(endPos, chunkSize);

                RegisterRoadInChunk(startCoord, road);
                if (!startCoord.Equals(endCoord))
                {
                    RegisterRoadInChunk(endCoord, road);
                }
            }
        }

        private void RegisterRoadInChunk(WorldChunkCoord coord, RealRoadSegment road)
        {
            if (!_chunkRoadMap.TryGetValue(coord, out var list))
            {
                list = new List<RealRoadSegment>();
                _chunkRoadMap[coord] = list;
            }

            if (!list.Contains(road))
            {
                list.Add(road);
            }
        }

        /// <summary>
        /// Retorna todas as vias pertencentes a uma célula espacial de streaming.
        /// </summary>
        public IReadOnlyList<RealRoadSegment> GetRoadsInChunk(WorldChunkCoord coord)
        {
            if (_chunkRoadMap.TryGetValue(coord, out var list))
            {
                return list;
            }
            return Array.Empty<RealRoadSegment>();
        }

        /// <summary>
        /// Localiza o segmento viário real mais próximo de uma coordenada Unity.
        /// </summary>
        public RealRoadSegment GetNearestRoad(Vector3 worldPos)
        {
            RealRoadSegment nearest = null;
            float nearestDistSqr = float.MaxValue;

            for (int i = 0; i < roadSegments.Count; i++)
            {
                var road = roadSegments[i];
                if (road == null) continue;

                Vector3 roadPos = road.transform.position;
                float distSqr = (roadPos - worldPos).sqrMagnitude;
                if (distSqr < nearestDistSqr)
                {
                    nearestDistSqr = distSqr;
                    nearest = road;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Registra manualmente um segmento viário na rede.
        /// </summary>
        public void RegisterRoad(RealRoadSegment segment)
        {
            if (segment != null && !roadSegments.Contains(segment))
            {
                roadSegments.Add(segment);
            }
        }

        /// <summary>
        /// Registra um ponto de interesse real de Araguari.
        /// </summary>
        public void RegisterPOI(GeoPointOfInterest poi)
        {
            if (poi != null && !pointsOfInterest.Contains(poi))
            {
                pointsOfInterest.Add(poi);
            }
        }
    }
}
