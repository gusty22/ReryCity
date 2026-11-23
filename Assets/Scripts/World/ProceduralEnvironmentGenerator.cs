using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Gerador modular determinístico de ambiente urbano (Procedural Environment System).
    /// Constrói quarteirões, vias, calçadas e lotes com base em uma semente inteira (Seed),
    /// garantindo que a mesma semente gere rigorosamente o mesmo mapa sem alocações em tempo de jogo.
    /// Registra os elementos diretamente nos WorldChunks do WorldStreamer.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/Procedural Environment Generator")]
    public class ProceduralEnvironmentGenerator : MonoBehaviour
    {
        public static ProceduralEnvironmentGenerator Instance { get; private set; }

        [Header("Semente e Determinismo")]
        [Tooltip("Semente mestre de geração. O mesmo valor produz o mesmo layout urbano.")]
        public int worldSeed = 192837;

        [Header("Dimensões do Grid Urbano")]
        [Range(1, 10)] public int gridDimensionX = 3;
        [Range(1, 10)] public int gridDimensionZ = 3;
        [Range(50f, 200f)] public float blockSpacing = 100f;

        [Header("Configurações de Quarteirão")]
        [SerializeField] private ProceduralBlockConfig residentialConfig;
        [SerializeField] private ProceduralBlockConfig commercialConfig;

        [Header("Estrutura Gerada")]
        [SerializeField] private bool generateOnStart = false;
        [SerializeField] private List<GameObject> generatedBlockRoots = new List<GameObject>();

        public int GeneratedBlockCount => generatedBlockRoots.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateWorldGrid(worldSeed);
            }
        }

        /// <summary>
        /// Gera uma malha urbana completa baseada em semente determinística.
        /// </summary>
        public void GenerateWorldGrid(int seed)
        {
            worldSeed = seed;
            ClearGeneratedWorld();

            System.Random prng = new System.Random(seed);
            var streamer = WorldStreamer.Instance;

            for (int x = -gridDimensionX / 2; x <= gridDimensionX / 2; x++)
            {
                for (int z = -gridDimensionZ / 2; z <= gridDimensionZ / 2; z++)
                {
                    Vector3 blockCenter = new Vector3(x * blockSpacing, 0f, z * blockSpacing);
                    WorldChunkCoord coord = new WorldChunkCoord(x, z);

                    // Seleciona configuração determinística por probabilidade da semente
                    bool isCommercial = prng.NextDouble() > 0.65;
                    ProceduralBlockConfig config = isCommercial ? commercialConfig : residentialConfig;

                    GameObject blockRoot = GenerateSingleBlock(coord, blockCenter, config, prng);
                    generatedBlockRoots.Add(blockRoot);

                    // Se o WorldStreamer existir, anexa o quarteirão ao chunk correspondente
                    if (streamer != null)
                    {
                        var chunk = streamer.GetChunk(coord);
                        if (chunk == null)
                        {
                            GameObject chunkGO = new GameObject($"Chunk_{coord.x}_{coord.z}");
                            chunk = chunkGO.AddComponent<WorldChunk>();
                            chunk.Initialize(coord, blockSpacing);
                            streamer.RegisterChunk(chunk);
                        }
                        chunk.RegisterEntity(blockRoot);
                    }
                }
            }
        }

        private GameObject GenerateSingleBlock(WorldChunkCoord coord, Vector3 center, ProceduralBlockConfig config, System.Random prng)
        {
            GameObject blockGO = new GameObject($"UrbanBlock_{coord.x}_{coord.z}");
            blockGO.transform.position = center;
            blockGO.transform.SetParent(transform, true);

            float bWidth = (config != null) ? config.blockSize.x : 80f;
            float bLength = (config != null) ? config.blockSize.y : 80f;
            int lotsX = (config != null) ? config.lotCountX : 2;
            int lotsZ = (config != null) ? config.lotCountZ : 2;

            float lotSizeX = bWidth / lotsX;
            float lotSizeZ = bLength / lotsZ;

            // Gera lotes e edifícios determinísticos
            for (int lx = 0; lx < lotsX; lx++)
            {
                for (int lz = 0; lz < lotsZ; lz++)
                {
                    float offsetX = -bWidth * 0.5f + (lx + 0.5f) * lotSizeX;
                    float offsetZ = -bLength * 0.5f + (lz + 0.5f) * lotSizeZ;
                    Vector3 lotPos = center + new Vector3(offsetX, 0f, offsetZ);

                    float minH = (config != null) ? config.buildingMinHeight : 6f;
                    float maxH = (config != null) ? config.buildingMaxHeight : 20f;
                    float height = minH + (float)(prng.NextDouble() * (maxH - minH));

                    GameObject building = CreateBuildingProxy(lotPos, new Vector3(lotSizeX * 0.85f, height, lotSizeZ * 0.85f), blockGO.transform);
                    building.name = $"Building_{lx}_{lz}";
                }
            }

            return blockGO;
        }

        private GameObject CreateBuildingProxy(Vector3 position, Vector3 size, Transform parent)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.transform.position = position + Vector3.up * (size.y * 0.5f);
            building.transform.localScale = size;
            building.transform.SetParent(parent, true);

            // Adiciona WorldLODGroup para controle de renderização por distância
            var lodGroup = building.AddComponent<WorldLODGroup>();
            lodGroup.InitializeDistances(35f, 80f, 150f);

            return building;
        }

        public void ClearGeneratedWorld()
        {
            for (int i = 0; i < generatedBlockRoots.Count; i++)
            {
                if (generatedBlockRoots[i] != null)
                {
                    DestroyImmediate(generatedBlockRoots[i]);
                }
            }
            generatedBlockRoots.Clear();
        }
    }
}
