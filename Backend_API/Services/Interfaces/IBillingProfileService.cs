using Backend_API.Data.Models;

namespace Backend_API.Services
{
    public interface IBillingProfileService
    {
        Task<string> CreateNewBillingProfileAsync(long customerID);

        Task<int> UpdateBillingProfileAsync(BillingProfile billingProfile);

        Task<int> DeactivateBillingProfileAsync(string billingProfileId);
    }
}
