using Models.DTO;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public interface IInteractionService
    {
        Task<ActionResult<CreateInteractionRS>> SaveNewInteractionAsync(InteractionDTO customerDTO);

        Task<ResponseBase> UpdateInteractionAsync(InteractionDTO interactionDTO);
    }
}
