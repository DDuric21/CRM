using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{ 
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
