using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.SeedData
{
    public class SeedData
    {
        /// <summary>
        /// Inserts SeedData into database
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <exception cref="Exception"></exception>
        public static void InsertSeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CrmDbContext>();

                try
                {
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                CreateCustomers(context);

                context.SaveChanges();
            }
        } 

        private static void CreateCustomers(CrmDbContext context)
        {
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(new List<Customer>
                {
                    new Customer
                    {
                        Name = "Test Name1",
                        Birthday = new DateTime(1995,05,16,0,0,0)
                    },
                    new Customer
                    {
                        Name = "Test Name2",
                        Birthday = new DateTime(1970,01,01,0,0,0)
                    },
                    new Customer
                    {
                        Name = "Test Name3",
                        Birthday = new DateTime(2000,03,21,0,0,0)
                    }
                });
            }
        }
    }
}
