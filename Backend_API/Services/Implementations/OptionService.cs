using AutoMapper;
using Backend_API.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Services
{
    public class OptionService : IOptionService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public OptionService(
            ICrmRepository crmRepository,
            IMapper mapper)
        {
            _repository = crmRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OptionDTO>> GetAssetAvailableOptionsAsync(long assetID)
        {
            var options = await _repository.Options
                .Where(x => x.AssetID == assetID)
                .ToListAsync();

            if (options.IsNullOrEmpty())
            {
                return new List<OptionDTO>();
            }

            var optionDTOs = options
                .Select(x => _mapper.Map<OptionDTO>(x))
                .ToList();

            return optionDTOs;
        }
    }
}
