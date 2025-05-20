using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public interface IBillingProfileService
    {
        Task<BillingProfileDTO> CreateNewBillingProfileAsync(long customerID);
        Task<bool> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO);
        Task<bool> DeactivateBillingProfileAsync(string billingProfileId);
    }
}
