using Backend_API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend_API.Data.DbContext
{
    public class CrmDbContext : Microsoft.EntityFrameworkCore.DbContext // for unknown reason i need to explicitly define DbContext
    {
        public static CrmDbContext _context;

        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
        {
            _context = this;
        }

        public static CrmDbContext CreateNewContext()
        {
            if (_context != null)
            {
                return _context;
            }
            else
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                var optionsBuilder = new DbContextOptionsBuilder();
                var options = (DbContextOptions<CrmDbContext>)optionsBuilder.UseSqlServer(connectionString).Options;

                _context = new CrmDbContext(options);

                return _context;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>();
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
