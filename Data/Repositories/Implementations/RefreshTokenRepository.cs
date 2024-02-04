using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
