using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public interface IBillingProfileRepository : IGenericRepository<BillingProfile>
    {
        long GetNextBillingProfileID();

        Task<BillingProfile> UpdateBillingProfileAsync(BillingProfile billingProfile);
    }
}
