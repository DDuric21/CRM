using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models.Authentication;
using Models.Requests;
using Models.Responses;
using Resources.Translations.API;

namespace Backend_API.Services
{
    public class ContactDetailsService : IContactDetailsService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public ContactDetailsService(
            ICrmRepository crmRepository,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _repository = crmRepository;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        public async Task<ResponseBase> EditContactDetailsAsync(EditContactDetailsRQ editContactDetailsRQ)
        {
            await _authorizationService.IsUserActionPermitted(editContactDetailsRQ.Username, CrmPermissionNames.EditContactDetails);

            if (editContactDetailsRQ.CustomerId == default)
            {
                throw new ArgumentException(APITranslations.InvalidCustomerIdProvided);
            }

            try
            { 
                var contactDetails = _mapper.Map<ContactDetails>(editContactDetailsRQ.CustomerContactDetails);
                contactDetails.CustomerId = editContactDetailsRQ.CustomerId;

                contactDetails.Id = _repository.ContactDetails
                    .Where(x => x.CustomerId == contactDetails.CustomerId)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                await _repository.ContactDetails.UpdateAsync(contactDetails);
                return new ResponseBase(true);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return new ResponseBase(false, $"An error occurred while updating contact details: {ex.Message}");
            }
        }
    }
}
