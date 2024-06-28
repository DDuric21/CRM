using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public interface ICrmRepository
    {
        ICustomerRepository Customers { get; }
        IAddressRepository Addresses { get; }
        IAssetRepository Assets { get; }
        ICustomerAssetsRepository CustomerAssets { get; }
        ICustomerAssetOptionsRepository CustomerAssetOptions { get; }
        IOptionRepository Options { get; }
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IGenericRepository<Interaction> Interactions { get; }
    }
}
