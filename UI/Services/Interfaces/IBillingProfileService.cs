using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public interface IBillingProfileService
    {
        Task<BillingProfileDTO> CreateNewBillingProfileAsync(long customerID);
        Task UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO);
    }
}
