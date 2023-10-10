namespace Backend_API.Data.Repositories
{
    public interface ICrmRepository
    {
        ICustomerRepository Customers { get; }
    }
}
