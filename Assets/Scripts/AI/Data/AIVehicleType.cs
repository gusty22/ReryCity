using System;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Categorias e classes de veículos civis e de serviço para tráfego urbano.
    /// Cada tipo dita dimensões físicas, dinâmica de frenagem, massa e comportamento no trânsito.
    /// </summary>
    public enum AIVehicleType
    {
        CivilMotorcycle,    // Motocicleta urbana (Titan, Fazer, CG) - ágil, inclina em curvas
        EconomyCar,         // Carro popular hatch (Gol, Uno, Kwid) - leve, comum
        Sedan,              // Sedã médio (Civic, Corolla) - porte médio, cruzeiro estável
        SUV,                // SUV compacto/médio (Compass, Renegade) - mais pesado, centro de massa mais alto
        Pickup,             // Caminhonete (Hilux, Toro) - longa, frenagem moderada
        Truck,              // Caminhão leve/médio de entregas - pesado, aceleração lenta, ampla distância de parada
        Bus,                // Ônibus urbano coletivo - muito longo, baixa agilidade, prioridade ao sair de pontos
        PoliceCruiser       // Viatura policial civil/militar - rápida, motor potente (preparação para Etapa 5.4)
    }
}
