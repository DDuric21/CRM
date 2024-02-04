namespace Backend_API.Data.Repositories
{
    public interface ICrmRepository
    {
        ICustomerRepository Customers { get; }
        IAddressRepository Addresses { get; }
        IAssetRepository Assets { get; }
        ICustomerAssetsRepository CustomerAssets { get; }
        IOptionRepository Options { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
    }
}
