using Models.DTO;

namespace UI.Services
{
    public interface IInteractionService
    {
        Task<long> SaveNewInteractionAsync(InteractionDTO customerDTO);

        Task<bool> UpdateInteractionAsync(InteractionDTO interactionDTO);
    }
}
