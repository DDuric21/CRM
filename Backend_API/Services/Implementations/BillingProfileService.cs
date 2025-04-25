using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Models.Enums;

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

        public async Task<string> CreateNewBillingProfileAsync(long customerID)
        {
            var customerType = _repository.Customers
                .Where(x => x.Id == customerID)
                .Select(x => x.TypeID)
                .FirstOrDefault();

            var nextBillingProfileID = _repository.BillingProfiles.GetNextBillingProfileID();

            var billingProfile = new BillingProfile { CustomerID = customerID };
            billingProfile.GenerateKey(customerType, nextBillingProfileID);

            await _repository.BillingProfiles.InsertAsync(billingProfile);

            return billingProfile.BillingProfileId;                
        }

        public async Task<int> UpdateBillingProfileAsync(BillingProfile billingProfile)
        {
            RemoveObjectsBeforeUpdate(billingProfile);

            return await _repository.BillingProfiles.UpdateBillingProfileAsync(billingProfile);
        }

        public async Task<int> DeactivateBillingProfileAsync(string billingProfileId)
        {
            var billingProfile = new BillingProfile(billingProfileId)
            {
                BillingProfileStatusID = (int)ItemState.Inactive
            };

            return await _repository.BillingProfiles.PartialUpdateAsync(billingProfile, x => x.BillingProfileStatusID);
        }

        private void RemoveObjectsBeforeUpdate(BillingProfile billingProfile)
        {
            billingProfile.Address = null;
            billingProfile.Customer = null;
        }
    }
}
