using Backend_API.Data.DbContext;
using Backend_API.Data.Models;
using Microsoft.EntityFrameworkCore;
using Models.Helpers;

namespace Backend_API.Data.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<int> BulkUpdate(List<Address> addresses, HashSet<long> addressIDs)
        {
            var existingAddresses = _context.Addresses
                .Where(x => addressIDs.Contains(x.Id))
                .ToList();

            if (existingAddresses.IsNullOrEmpty())
            {
                return 0;
            }

            foreach (var address in existingAddresses)
            {
                _context.Entry(address).CurrentValues.SetValues(addresses.FirstOrDefault(x => x.Id == address.Id));
                _context.Entry(address).State = EntityState.Modified;
            }

            return await SaveAsync();
        }
    }
}
