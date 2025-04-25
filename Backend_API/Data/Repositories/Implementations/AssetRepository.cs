using Backend_API.Data.DbContext;
using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public class AssetRepository : GenericRepository<Asset>, IAssetRepository
    {
        public AssetRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
