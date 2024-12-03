using Backend_API.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_API.Data.DbContext
{
    public class CrmDbContext : IdentityDbContext
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
            DefineCreationDate(modelBuilder);

            DefineModelBuilderEntities(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void DefineCreationDate(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ITrackChanges).IsAssignableFrom(e.ClrType));

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("DateCreated")
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }

        private static void DefineModelBuilderEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>();

            modelBuilder.Entity<Customer>()
                .HasMany(x => x.Addresses)
                .WithOne(x => x.Customer);

            modelBuilder.Entity<User>();

            modelBuilder.Entity<Asset>()
                .HasMany(x => x.Options)
                .WithOne(x => x.Asset);

            modelBuilder.Entity<CustomerAssetOptions>()
                .HasKey(x => new { x.CustomerAssetsID, x.OptionID });
            modelBuilder.Entity<CustomerAssetOptions>()
                .HasOne(x => x.CustomerAssets)
                .WithMany(x => x.CustomerAssetOptions);
            modelBuilder.Entity<CustomerAssetOptions>()
                .HasOne(x => x.Option)
                .WithMany(x => x.CustomerAssetOptions);

            modelBuilder.Entity<Option>();

            modelBuilder.Entity<RefreshToken>();

            modelBuilder.Entity<Order>()
                .HasOne(x => x.CustomerAssets)
                .WithMany(x => x.Orders);
            modelBuilder.Entity<Order>()
                .Property(x => x.CustomerAssetsID)
                .IsRequired(false);

            modelBuilder.Entity<Interaction>();

            modelBuilder.Entity<BillingProfile>()
                .HasCheckConstraint("CK_BillingProfile_Key_Format", "BillingProfileId LIKE '[0-9]-%[0-9]'");
            modelBuilder.Entity<BillingProfile>()
                .HasOne(x => x.Address)
                .WithMany(x => x.BillingProfiles)
                .HasForeignKey(x => x.AddressID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<ITrackChanges>()
                .Where(x => x.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                entityEntry.Entity.DateModified = DateTime.UtcNow;
                entityEntry.Property(nameof(ITrackChanges.DateCreated)).IsModified = false;
            }
        }

        #region DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<CustomerAssets> CustomerAssets { get; set; }
        public DbSet<CustomerAssetOptions> CustomerAssetOptions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<BillingProfile> BillingProfiles { get; set; }
        #endregion
    }
}
