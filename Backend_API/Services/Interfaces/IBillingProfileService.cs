using Backend_API.Data.Model;

namespace Backend_API.Services
{
    public interface IBillingProfileService
    {
        Task<string> CreateNewBillingProfileAsync(long customerID);
        Task<int> UpdateBillingProfileAsync(BillingProfile billingProfile);
    }
}
