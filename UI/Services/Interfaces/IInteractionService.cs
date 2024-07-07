using Models.DTO;

namespace UI.Services
{
    public interface IInteractionService
    {
        Task<long> SaveNewInteractionAsync(InteractionDTO customerDTO);

        Task UpdateInteractionAsync(InteractionDTO interactionDTO);
    }
}
