using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Models.DTO;

namespace Backend_API.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public InteractionService(
            ICrmRepository crmRepository,
            IMapper mapper)
        {
            _repository = crmRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateInteractionAsync(InteractionDTO interactionDTO)
        {
            try
            {
                var interaction = _mapper.Map<Interaction>(interactionDTO);
                await _repository.Interactions.InsertAsync(interaction);

                return interaction.Id;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateInteractionAsync(InteractionDTO interaction)
        {
            try
            {
                var interactionEntity = _mapper.Map<Interaction>(interaction);
                return await _repository.Interactions.UpdateAsync(interactionEntity);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                throw;
            }
        }
    }
}
