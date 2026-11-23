using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Representação de uma célula discreta (Chunk) do mundo urbano.
    /// Contém e gerencia a hierarquia de objetos, quarteirões, calçadas, edifícios e postes,
    /// permitindo ativação e desativação sem Garbage Collection.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/World Chunk")]
    public class WorldChunk : MonoBehaviour
    {
        [Header("Identificação da Célula")]
        [SerializeField] private WorldChunkCoord coord;
        [SerializeField] private float chunkSize = 100f;
        [SerializeField] private bool isLoaded = false;
        [SerializeField] private bool isVisible = true;

        [Header("Elementos Contidos")]
        [SerializeField] private List<GameObject> chunkEntities = new List<GameObject>();

        public WorldChunkCoord Coord => coord;
        public float ChunkSize => chunkSize;
        public bool IsLoaded => isLoaded;
        public bool IsVisible => isVisible;
        public Vector3 CenterPosition => coord.ToCenterWorldPosition(chunkSize, transform.position.y);

        public void Initialize(WorldChunkCoord chunkCoord, float size)
        {
            coord = chunkCoord;
            chunkSize = size;
            transform.position = new Vector3(chunkCoord.x * size, 0f, chunkCoord.z * size);
            gameObject.name = $"Chunk_{chunkCoord.x}_{chunkCoord.z}";
        }

        public void RegisterEntity(GameObject entity)
        {
            if (entity != null && !chunkEntities.Contains(entity))
            {
                chunkEntities.Add(entity);
                entity.transform.SetParent(transform, true);
            }
        }

        public void Load()
        {
            if (isLoaded) return;
            isLoaded = true;
            gameObject.SetActive(true);
            SetEntitiesActive(true);
        }

        public void Unload()
        {
            if (!isLoaded) return;
            isLoaded = false;
            SetEntitiesActive(false);
            gameObject.SetActive(false);
        }

        public void SetVisible(bool visible)
        {
            isVisible = visible;
            SetEntitiesActive(visible);
        }

        private void SetEntitiesActive(bool active)
        {
            for (int i = 0; i < chunkEntities.Count; i++)
            {
                if (chunkEntities[i] != null)
                {
                    chunkEntities[i].SetActive(active);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isLoaded ? Color.green : Color.gray;
            Vector3 center = CenterPosition;
            center.y = 10f;
            Gizmos.DrawWireCube(center, new Vector3(chunkSize, 20f, chunkSize));
        }
    }
}
