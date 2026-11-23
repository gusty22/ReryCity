using UnityEngine;

namespace ProjetoGrau2.Interaction
{
    public enum InteractionType
    {
        EntrarEstabelecimento,
        AbastecerCombustivel,
        ManutencaoMoto,
        ComprarPeca,
        ConversarNPC,
        ServicoBorracharia
    }

    /// <summary>
    /// Contrato universal para qualquer entidade, ponto de serviço ou estabelecimento interativo em RERY CITY.
    /// </summary>
    public interface IInteractable
    {
        string InteractionId { get; }
        string PromptText { get; }
        string InteractionPrompt => PromptText;
        InteractionType Type { get; }
        float MaxInteractionDistance { get; }
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
        void CancelInteraction(GameObject interactor);
    }
}
