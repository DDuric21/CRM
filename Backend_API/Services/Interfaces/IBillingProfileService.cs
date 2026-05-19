using Models.DTO;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IBillingProfileService
    {
        Task<CreateNewBillingProfileRS> CreateNewBillingProfileAsync(long customerID);

        Task<UpdateBillingProfileRS> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO);

        Task<ResponseBase> DeactivateBillingProfileAsync(string billingProfileId);
    }
}
