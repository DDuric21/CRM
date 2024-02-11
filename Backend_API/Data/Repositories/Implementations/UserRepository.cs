using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{ 
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CrmDbContext context) : base(context)
        {
        }

        public User GetUserByEmail(string userEmail)
        {
            return _context.Users
                .Where(x => x.UserEmail.ToLower() == userEmail.ToLower())
                .FirstOrDefault();
        }
    }
}
