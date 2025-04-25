using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
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

        public async Task CreateInteractionAsync(Interaction interaction)
        {
            await _repository.Interactions.InsertAsync(interaction);
        }

        public async Task<int> UpdateInteractionAsync(Interaction interaction)
        {
            return await _repository.Interactions.UpdateAsync(interaction);
        }

        public InteractionDTO MapToDTO(Interaction interaction)
        {
            var interactionDTO = _mapper.Map<InteractionDTO>(interaction);

            return interactionDTO;
        }

        public Interaction MapDtoToInteraction(InteractionDTO interactionDTO)
        {
            var interaction = _mapper.Map<Interaction>(interactionDTO);

            return interaction;
        }
    }
}
