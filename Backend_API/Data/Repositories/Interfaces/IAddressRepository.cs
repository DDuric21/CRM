using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<int> BulkUpdate(List<Address> addresses, HashSet<long> addressIDs);
    }
}
