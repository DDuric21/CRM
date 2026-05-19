using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public interface ICustomerAssetsRepository : IGenericRepository<CustomerAssets>
    {
        Task<int> UpdateCustomerAssetDataAsync(CustomerAssets customerAssets);
    }
}
