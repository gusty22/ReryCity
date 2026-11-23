using System;
using UnityEngine;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Coordenada bidimensional inteira de uma célula discreta (Chunk) na grade do mundo aberto.
    /// Utiliza hashing otimizado para dicionários sem alocação de memória no heap.
    /// </summary>
    [Serializable]
    public struct WorldChunkCoord : IEquatable<WorldChunkCoord>
    {
        public int x;
        public int z;

        public WorldChunkCoord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public bool Equals(WorldChunkCoord other)
        {
            return x == other.x && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldChunkCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ z;
            }
        }

        public static bool operator ==(WorldChunkCoord left, WorldChunkCoord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldChunkCoord left, WorldChunkCoord right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({x}, {z})";
        }

        public static WorldChunkCoord FromWorldPosition(Vector3 position, float chunkSize)
        {
            int cx = Mathf.FloorToInt(position.x / chunkSize);
            int cz = Mathf.FloorToInt(position.z / chunkSize);
            return new WorldChunkCoord(cx, cz);
        }

        public Vector3 ToCenterWorldPosition(float chunkSize, float y = 0f)
        {
            float wx = (x + 0.5f) * chunkSize;
            float wz = (z + 0.5f) * chunkSize;
            return new Vector3(wx, y, wz);
        }
    }
}
