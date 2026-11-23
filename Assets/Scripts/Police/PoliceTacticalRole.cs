using System;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Papéis táticos atribuídos a unidades policiais em uma perseguição coordenada multi-viatura.
    /// Garante que as viaturas não se aglomerem no mesmo ponto e desempenhem funções especializadas.
    /// </summary>
    public enum PoliceTacticalRole
    {
        None = 0,

        /// <summary>
        /// Viatura líder: persegue diretamente na esteira do suspeito (8 a 15m).
        /// Atribuído preferencialmente a motocicletas ROCAM devido à agilidade.
        /// </summary>
        LeadPursuer = 1,

        /// <summary>
        /// Flanqueador esquerdo: posiciona-se lateralmente à esquerda para conter desvios ou ultrapassagens.
        /// </summary>
        FlankerLeft = 2,

        /// <summary>
        /// Flanqueador direito: posiciona-se lateralmente à direita para fechar o corredor de escape.
        /// </summary>
        FlankerRight = 3,

        /// <summary>
        /// Interceptador frontal: viatura pesada que prevê o trajeto e busca ultrapassar/cortar a rota à frente.
        /// </summary>
        Interceptor = 4,

        /// <summary>
        /// Varredura perimetral: unidade destacada para investigar quadrantes específicos da LKP após perda de visão.
        /// </summary>
        PerimeterSearch = 5,

        /// <summary>
        /// Apoio de contenção: retaguarda tática pronta para assumir a liderança em caso de despiste.
        /// </summary>
        Support = 6
    }
}
