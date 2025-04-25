using Backend_API.Data.Models;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IInteractionService
    {
        InteractionDTO MapToDTO(Interaction interaction);
        Interaction MapDtoToInteraction(InteractionDTO interactionDTO);

        Task CreateInteractionAsync(Interaction interaction);
        Task<int> UpdateInteractionAsync(Interaction interaction);
    }
}
