using Backend_API.Data.DbContext;
using Backend_API.Data.Model;
using Ng.Services;

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

                CreateSeedData(context);

                context.SaveChanges();
            }
        }

        private static void CreateSeedData(CrmDbContext context)
        {
            CreateAddresses(context);

            CreateCustomers(context);

            CreateAssets(context);

            CreateUsers(context);

            CreateCustomerAssets(context);

            CreateOptions(context);
        }

        private static void CreateAddresses(CrmDbContext context)
        {
            if (!context.Addresses.Any())
            {
                context.Addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        FullAddress = "Ulica Grada Chicaga 33, 10000, Zagreb"
                    },
                    new Address
                    {
                        FullAddress = "Trg Bana Josipa Jelačića 1, 10000, Zagreb"
                    },
                    new Address
                    {
                        FullAddress = "Avenija Dubrovnik 24, 10000, Zagreb"
                    }
                });
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
                        AddressId = 1,
                        Birthday = new DateTime(1995,05,16,0,0,0)
                    },
                    new Customer
                    {
                        Name = "Test Name2",
                        AddressId = 2,
                        Birthday = new DateTime(1970,01,01,0,0,0)
                    },
                    new Customer
                    {
                        Name = "Test Name3",
                        AddressId = 3,
                        Birthday = new DateTime(2000,03,21,0,0,0)
                    }
                });
            }
        }

        private static void CreateAssets(CrmDbContext context)
        {
            if (!context.Assets.Any())
            {
                context.AddRange(new List<Asset>
                {
                    new Asset
                    {
                        Name = "Mobilna Voice Usluga",
                        Price = 100,
                        CurrencyID = 0
                    },
                    new Asset
                    {
                        Name = "Fiksna Usluga",
                        Price = 150,
                        CurrencyID = 0
                    },
                    new Asset
                    {
                        Name = "Najam Opreme",
                        Price = 20,
                        CurrencyID = 0
                    },
                });
            }
        }

        private static void CreateUsers(CrmDbContext context)
        {
            if (!context.Users.Any())
            {
                var passwordHashingService = new PasswordHashingService();
                context.Users.AddRange(new List<User>
                {
                    new User
                    {
                        UserName = "Pero Perić",
                        UserEmail = "pero.peric@nepostoji.rh",
                        Password = passwordHashingService.HashPassword("readOnly"),
                        UserRoleId = 2
                    },
                    new User
                    {
                        UserName = "Ivo Ivić",
                        UserEmail = "ivo.ivic@nepostoji.rh",
                        Password = passwordHashingService.HashPassword("edit"),
                        UserRoleId = 1
                    },
                    new User
                    {
                        UserName = "Admin Adminić",
                        UserEmail = "admin.adminic@nepostoji.rh",
                        Password = passwordHashingService.HashPassword("admin"),
                        UserRoleId = 0
                    }
                });
            }
        }

        private static void CreateCustomerAssets(CrmDbContext context)
        {
            if (!context.CustomerAssets.Any())
            {
                context.CustomerAssets.AddRange(new List<CustomerAssets>
                {
                    new CustomerAssets
                    {
                        CustomerID = 1,
                        AssetID = 1
                    },
                    new CustomerAssets
                    {
                        CustomerID = 1,
                        AssetID = 2
                    },
                    new CustomerAssets
                    {
                        CustomerID = 2,
                        AssetID = 2
                    },
                    new CustomerAssets
                    {
                        CustomerID = 3,
                        AssetID = 1
                    },
                    new CustomerAssets
                    {
                        CustomerID = 3,
                        AssetID = 3
                    }
                });
            }
        }

        private static void CreateOptions(CrmDbContext context)
        {
            if (!context.Options.Any())
            {
                context.Options.AddRange(new List<Option>
                {
                    new Option
                    {
                        Name = "+1 GB",
                        Price = 20,
                        CurrencyID = 0,
                        AssetID = 1
                    },
                    new Option
                    {
                        Name = "+5 GB",
                        Price = 80,
                        CurrencyID = 0,
                        AssetID = 1
                    },
                    new Option
                    {
                        Name = "Brzina 100 MB/s",
                        Price = 15,
                        CurrencyID = 0,
                        AssetID = 2
                    },
                    new Option
                    {
                        Name = "Brzina 200 MB/s",
                        Price = 50,
                        CurrencyID = 0,
                        AssetID = 2
                    },
                    new Option
                    {
                        Name = "Huawei mash paket",
                        Price = 350,
                        CurrencyID = 0,
                        AssetID = 2
                    },
                });
            }
        }
    }
}
