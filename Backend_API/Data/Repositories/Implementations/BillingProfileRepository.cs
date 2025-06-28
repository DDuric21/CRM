using Backend_API.Data.DbContext;
using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public class BillingProfileRepository : GenericRepository<BillingProfile>, IBillingProfileRepository
    {
        public BillingProfileRepository(CrmDbContext context) : base(context)
        {
        }

        public long GetNextBillingProfileID()
        {
            var nextID = _context.BillingProfiles.Count();

            return nextID;
        }

        public async Task<BillingProfile> UpdateBillingProfileAsync(BillingProfile billingProfile)
        {
            var existingProfile = _context.BillingProfiles.FirstOrDefault(x => x.BillingProfileId == billingProfile.BillingProfileId);
            
            if (existingProfile is null)
            {
                return null;
            }

            _context.Entry(existingProfile).CurrentValues.SetValues(billingProfile);

            await SaveAsync();

            return existingProfile;
        }
    }
}
