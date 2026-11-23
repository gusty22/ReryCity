using System;

namespace ProjetoGrau2.World
{
    /// <summary>
    /// Categorias de acontecimentos e ocorrências dinâmicas no mundo urbano.
    /// </summary>
    public enum WorldEventType
    {
        TrafficAccident,    // Acidente de trânsito (colisão entre veículos NPC)
        RoadworkBlockage,   // Obras viárias com cones, barreiras e desvio obrigatório
        BrokenVehicle,      // Veículo civil com pane mecânica (pisca-alerta/parado)
        PoliceCheckpoint,   // Bloqueio / blitz policial com abordagem
        HazardObstacle      // Queda de carga, detrito ou obstáculo na pista
    }

    /// <summary>
    /// Ciclo de vida operacional de uma ocorrência dinâmica no mundo.
    /// </summary>
    public enum WorldEventState
    {
        Pending,        // Criado, aguardando ativação ou posicionamento
        Active,         // Em andamento ativo na malha viária
        Resolving,      // Em atendimento (polícia/socorro no local)
        Completed,      // Concluído e liberado para limpeza/pool
        Cancelled       // Descartado por distância ou sobreposição
    }
}
