using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    public enum BuildingStyleCategory
    {
        ResidenciaPopular,     // Casas geminadas / bairros periféricos
        CasaPadraoMedio,       // Casas térreas com garagem e muro
        ResidenciaModerna,     // Arquitetura contemporânea / 2 pavimentos
        EdificioComercial,     // Lojas térreas com salas no piso superior
        PredioResidencial,     // Edifícios de 3 a 8 andares (típicos do Centro)
        OficinaMecanicaGrau,   // Oficinas de preparação de motos e escapamentos
        PostoCombustivel,      // Postos urbanos com cobertura metálica
        MarcoHistoricoPublico  // Palácio dos Ferroviários, Mercado, Igreja Matriz
    }

    /// <summary>
    /// Representa um lote urbano delimitado no grid de Araguari.
    /// Define dimensões, recuos frontais em relação à calçada e zoneamento edilício.
    /// </summary>
    [System.Serializable]
    public class UrbanLot
    {
        public string lotId;
        public Vector3 centerPosition;
        public float frontageMeters;    // Testada do lote (largura frontal)
        public float depthMeters;       // Profundidade do lote
        public float rotationY;         // Orientação em relação à via
        public BuildingStyleCategory styleCategory;
        public float frontSetback;      // Recuo frontal obrigatório da via

        public Bounds GetLotBounds()
        {
            Vector3 size = new Vector3(frontageMeters, 20f, depthMeters);
            return new Bounds(centerPosition, size);
        }

        /// <summary>
        /// Verifica se o lote invade a caixa viária dada a meia-largura da via e da calçada.
        /// </summary>
        public bool IsClearOfRoad(Vector3 roadCenter, float roadHalfWidth, float sidewalkWidth)
        {
            float totalRoadCorridor = roadHalfWidth + sidewalkWidth;
            float distanceToRoadCenter = Vector3.Distance(new Vector3(centerPosition.x, 0f, centerPosition.z),
                                                          new Vector3(roadCenter.x, 0f, roadCenter.z));

            float halfLotExtent = Mathf.Min(frontageMeters, depthMeters) * 0.5f;
            return (distanceToRoadCenter - halfLotExtent) >= (totalRoadCorridor + frontSetback);
        }
    }
}
