using Backend_API.Data.Models;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IInteractionService
    {
        Task<long> CreateInteractionAsync(InteractionDTO interaction);

        Task<int> UpdateInteractionAsync(InteractionDTO interaction);
    }
}
