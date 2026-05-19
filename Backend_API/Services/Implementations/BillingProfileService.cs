using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Models.DTO;
using Models.Enums;
using Models.Responses;
using Resources.Translations.API;

namespace Backend_API.Services
{
    public class BillingProfileService : IBillingProfileService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public BillingProfileService(
            ICrmRepository crmRepository,
            IMapper mapper)
        {
            _repository = crmRepository;
            _mapper = mapper;
        }

        public async Task<CreateNewBillingProfileRS> CreateNewBillingProfileAsync(long customerID)
        {
            var billingProfile = BuildNewBillingProfile(customerID);

            try
            {
                await _repository.BillingProfiles.InsertAsync(billingProfile);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Failed to create a new billing profile.");
                throw new Exception(APITranslations.BillingProfileNotCreated);
            }

            var response = new CreateNewBillingProfileRS
            {
                BillingProfile = _mapper.Map<BillingProfileDTO>(billingProfile),
                IsSuccess = true
            };

            return response;
        }

        private BillingProfile BuildNewBillingProfile(long customerID)
        {
            var customerType = _repository.Customers
                .Where(x => x.Id == customerID)
                .Select(x => x.TypeID)
                .FirstOrDefault();

            var nextBillingProfileID = _repository.BillingProfiles.GetNextBillingProfileID();

            var billingProfile = new BillingProfile { CustomerID = customerID };
            billingProfile.GenerateKey(customerType, nextBillingProfileID);

            return billingProfile;
        }

        public async Task<UpdateBillingProfileRS> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO)
        {
            var billingProfile = _mapper.Map<BillingProfile>(billingProfileDTO);

            if (billingProfile.AddressID > 0
                && billingProfile.BillingProfileStatusID == (int)ItemState.Incomplete)
            {
                billingProfile.BillingProfileStatusID = (int)ItemState.Active;
            }

            var updatedProfile = await _repository.BillingProfiles.UpdateBillingProfileAsync(billingProfile);

            if (updatedProfile  is null)
            {
                DynamicLogger.LogError($"Failed to update billing profile with ID: {billingProfile.BillingProfileId}");
                return new UpdateBillingProfileRS { ErrorMessage = APITranslations.BillingProfileNotUpdated };
            }

            var updatedBillingProfileDTO = _mapper.Map<BillingProfileDTO>(updatedProfile);

            return new UpdateBillingProfileRS(updatedBillingProfileDTO);
        }

        public async Task<ResponseBase> DeactivateBillingProfileAsync(string billingProfileId)
        {
            var billingProfile = new BillingProfile(billingProfileId)
            {
                BillingProfileStatusID = (int)ItemState.Inactive
            };

            var updatedCount =  await _repository.BillingProfiles.PartialUpdateAsync(billingProfile, x => x.BillingProfileStatusID);
            var isSuccess = updatedCount > 0;

            if (!isSuccess)
            {
                DynamicLogger.LogError($"Failed to deactivate billing profile with ID: {billingProfileId}");
                return new ResponseBase(false, APITranslations.BillingProfileNotDeactivated);
            }

            return new ResponseBase(isSuccess);
        }
    }
}
