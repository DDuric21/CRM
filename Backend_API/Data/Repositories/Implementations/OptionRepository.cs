using Backend_API.Data.DbContext;
using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public class OptionRepository : GenericRepository<Option>, IOptionRepository
    {
        public OptionRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
