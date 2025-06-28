using Backend_API.Data.DbContext;
using Backend_API.Data.Models;
using Backend_API.Services;
using Microsoft.AspNetCore.Identity;
using Models.Authentication;
using System.Security.Claims;

namespace Backend_API.Data.SeedData
{
    public class SeedData
    {
        private readonly CrmUserManager _userManager;
        private readonly CrmRoleManager _roleManager;
        private const string _adminRole = "admin";
        private const string _userRole = "user";
        private const string _testRole = "test";

        public SeedData(
            CrmUserManager userManager,
            CrmRoleManager roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Inserts SeedData into database
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <exception cref="Exception"></exception>
        public async static void InsertSeedData(IApplicationBuilder applicationBuilder)
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
                await CreateRolesAsync(serviceScope);
                await CreateUsersAsync(serviceScope);

                context.SaveChanges();
            }
        }

        private static void CreateSeedData(CrmDbContext context)
        {
            CreateCustomers(context);

            CreateAddresses(context);

            CreateAssets(context);

            CreateOptions(context);

            CreateBillingProfiles(context);

            CreateCustomerAssets(context);

            CreateCustomerAssetOptions(context);

            CreateCustomerInteractions(context);

            CreateNews(context);
        }

        private static void CreateCustomers(CrmDbContext context)
        {
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(new List<Customer>
                {
                    new Customer
                    {
                        FirstName = "Test",
                        LastName =  "Name1",
                        PersonalID =  "04766934440",
                        Birthday = new DateTime(1995,05,16,0,0,0),
                        TypeID = 1,
                        CustomerStatusID = 1,
                    },
                    new Customer
                    {
                        FirstName = "Test",
                        LastName =  "Name2",
                        PersonalID =  "54745617737",
                        Birthday = new DateTime(1970,01,01,0,0,0),
                        TypeID = 1,
                        CustomerStatusID = 1,
                    },
                    new Customer
                    {
                        FirstName = "Test",
                        LastName =  "Name3",
                        PersonalID =  "37046495774",
                        Birthday = new DateTime(2000,03,21,0,0,0),
                        TypeID = 1,
                        CustomerStatusID = 2,
                    }
                });

                //needed so that FK constraints don't appear
                context.SaveChanges();
            }
        }

        private static void CreateAddresses(CrmDbContext context)
        {
            if (!context.Addresses.Any())
            {
                context.Addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        CustomerId = 1,
                        IsLegal = true,
                        FullAddress = "Ulica Grada Chicaga 33, 10000, Zagreb"
                    },
                    new Address
                    {
                        CustomerId = 2,
                        IsLegal = true,
                        FullAddress = "Trg Bana Josipa Jelačića 1, 10000, Zagreb"
                    },
                    new Address
                    {
                        CustomerId = 2,
                        FullAddress = "Avenija Dubrovnik 24, 10000, Zagreb"
                    },
                    new Address
                    {
                        CustomerId = 3,
                        FullAddress = "Trg Bana Josipa Jelačića 1, 10000, Zagreb"
                    },
                    new Address
                    {
                        CustomerId = 3,
                        FullAddress = "Avenija Dubrovnik 24, 10000, Zagreb"
                    },
                    new Address
                    {
                        CustomerId = 3,
                        IsLegal = true,
                        FullAddress = "Ulica Grada Chicaga 33, 10000, Zagreb"
                    }
                });

                //needed so that FK constraints don't appear
                context.SaveChanges();
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

                //needed so that FK constraints don't appear
                context.SaveChanges();
            }
        }

        private async static Task CreateRolesAsync(IServiceScope scope)
        {
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<CrmRoleManager>();

            if (roleManager.Roles.Any())
            {
                return;
            }

            await roleManager.CreateAsync(new IdentityRole(_adminRole));
            await CreateAdminRoleClaimsAsync(roleManager);
            await roleManager.CreateAsync(new IdentityRole(_userRole));
            await CreateUserRoleClaimsAsync(roleManager);
            await roleManager.CreateAsync(new IdentityRole(_testRole));
            await CreateUserTestClaimsAsync(roleManager);
        }

        private async static Task CreateUsersAsync(IServiceScope scope)
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();

            if (userManager.Users.Any())
            {
                return;
            }

            var testUser = new User
            {
                FirstName = "Pero",
                LastName = "Perić",
                UserName = "pero.peric",
                Email = "pero.peric@test.com",
                UserStatusID = 2
            };
            testUser.PasswordHash = userManager.PasswordHasher.HashPassword(testUser, "test");
            var testUserResult = await userManager.CreateAsync(testUser);
            if (testUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, _testRole);
            }

            var userUser = new User
            {
                FirstName = "Ivo",
                LastName = "Ivic",
                UserName = "ivo.ivic",
                Email = "ivo.ivic@user.com",
                UserStatusID = 1
            };
            userUser.PasswordHash = userManager.PasswordHasher.HashPassword(userUser, "user");
            var userResult = await userManager.CreateAsync(userUser);
            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(userUser, _userRole);
            }

            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "Adminić",
                UserName = "admin.adminic",
                Email = "admin.adminic@admin.com",
                UserStatusID = 1
            };
            adminUser.PasswordHash = userManager.PasswordHasher.HashPassword(adminUser, "admin");
            var admiResult = await userManager.CreateAsync(adminUser);
            if (admiResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }
        }

        private static async Task CreateAdminRoleClaimsAsync(CrmRoleManager roleManager)
        {
            var adminPermissions = new[]
            {
                CrmPermissionNames.ReadUser,
                CrmPermissionNames.EditUser,
                CrmPermissionNames.CreateUser,
                CrmPermissionNames.DeleteUser,
                CrmPermissionNames.ReadCustomer,
                CrmPermissionNames.EditCustomer,
                CrmPermissionNames.DeleteCustomer,
                CrmPermissionNames.CreateCustomer,
                CrmPermissionNames.ReadAsset,
                CrmPermissionNames.EditAsset,
                CrmPermissionNames.DeleteAsset,
                CrmPermissionNames.CreateAsset
            };

            var adminRole = roleManager.Roles.FirstOrDefault(x => x.Name == _adminRole);

            if (adminRole is null)
            {
                return;
            }

            foreach (var permission in adminPermissions)
            {
                await roleManager.AddClaimAsync(adminRole, new Claim(CrmJwtClaimNames.Permission, permission));
            }
        }

        private static async Task CreateUserRoleClaimsAsync(CrmRoleManager roleManager)
        {
            var userPermissions = new[]
            {
                CrmPermissionNames.ReadUser,
                CrmPermissionNames.EditUser,
                CrmPermissionNames.CreateUser,
                CrmPermissionNames.ReadCustomer,
                CrmPermissionNames.EditCustomer,
                CrmPermissionNames.CreateCustomer,
                CrmPermissionNames.ReadAsset,
                CrmPermissionNames.EditAsset,
                CrmPermissionNames.CreateAsset
            };

            var userRole = roleManager.Roles.FirstOrDefault(x => x.Name == _userRole);

            if (userRole is null)
            {
                return;
            }

            foreach (var permission in userPermissions)
            {
                await roleManager.AddClaimAsync(userRole, new Claim(CrmJwtClaimNames.Permission, permission));
            }
        }

        private static async Task CreateUserTestClaimsAsync(CrmRoleManager roleManager)
        {
            var testPermissions = new[]
            {
                CrmPermissionNames.ReadUser,
                CrmPermissionNames.ReadCustomer,
                CrmPermissionNames.ReadAsset
            };

            var testRole = roleManager.Roles.FirstOrDefault(x => x.Name == _testRole);

            if (testRole is null)
            {
                return;
            }

            foreach (var permission in testPermissions)
            {
                await roleManager.AddClaimAsync(testRole, new Claim(CrmJwtClaimNames.Permission, permission));
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

                //needed so that FK constraints dont appear
                context.SaveChanges();
            }
        }

        private static void CreateBillingProfiles(CrmDbContext context)
        {
            if (!context.BillingProfiles.Any())
            {
                var billingProfile1 = new BillingProfile { CustomerID = 1, AddressID = 1, BillingProfileStatusID = 1, Paperless = true };
                billingProfile1.GenerateKey(1, 123);
                var billingProfile2 = new BillingProfile { CustomerID = 2, AddressID = 2, BillingProfileStatusID = 1 };
                billingProfile2.GenerateKey(1, 456);
                var billingProfile3 = new BillingProfile { CustomerID = 2, AddressID = 3, BillingProfileStatusID = 2 };
                billingProfile3.GenerateKey(1, 789);
                var billingProfile4 = new BillingProfile { CustomerID = 2, AddressID = 2, BillingProfileStatusID = 3 };
                billingProfile4.GenerateKey(1, 987);
                var billingProfile5 = new BillingProfile { CustomerID = 3, AddressID = 5, BillingProfileStatusID = 1 };
                billingProfile5.GenerateKey(1, 321);

                context.BillingProfiles.AddRange(new List<BillingProfile>
                {
                    billingProfile1,
                    billingProfile2,
                    billingProfile3,
                    billingProfile4,
                    billingProfile5
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
                        AssetID = 1,
                        AssetAddressID = 1,
                        AssetStatusID = 1,
                        BillingProfileId = "1-123"
                    },
                    new CustomerAssets
                    {
                        CustomerID = 1,
                        AssetID = 1,
                        AssetAddressID = 1,
                        AssetStatusID = 2,
                        BillingProfileId = "1-123"
                    },
                    new CustomerAssets
                    {
                        CustomerID = 1,
                        AssetID = 2,
                        AssetAddressID = 1,
                        AssetStatusID = 1,
                        BillingProfileId = "1-123"
                    },
                    new CustomerAssets
                    {
                        CustomerID = 2,
                        AssetID = 2,
                        AssetAddressID = 2,
                        AssetStatusID = 1,
                        BillingProfileId = "1-456"
                    },
                    new CustomerAssets
                    {
                        CustomerID = 3,
                        AssetID = 1,
                        AssetAddressID = 4,
                        AssetStatusID = 1,
                        BillingProfileId = "1-321"
                    },
                    new CustomerAssets
                    {
                        CustomerID = 3,
                        AssetID = 3,
                        AssetAddressID = 5,
                        AssetStatusID = 1,
                        BillingProfileId = "1-321"
                    }
                });

                context.SaveChanges();
            }
        }

        private static void CreateCustomerAssetOptions(CrmDbContext context)
        {
            if (!context.CustomerAssetOptions.Any())
            {
                context.CustomerAssetOptions.AddRange(new List<CustomerAssetOptions>
                {
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 1,
                        OptionID = 1,
                    },
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 2,
                        OptionID = 3,
                    },
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 2,
                        OptionID = 5,
                    },
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 3,
                        OptionID = 4,
                    },
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 3,
                        OptionID = 3,
                    },
                    new CustomerAssetOptions
                    {
                        CustomerAssetsID = 4,
                        OptionID = 3,
                    }
                });
            }
        }

        private static void CreateCustomerInteractions(CrmDbContext context)
        {
            if (!context.Interactions.Any())
            {
                context.Interactions.AddRange(new List<Interaction>
                {
                    new Interaction
                    {
                        CustomerID = 1,
                        DateTime = new DateTime(1995,05,16,0,0,0),
                        TypeID = 3,
                        Description = "My first Complaint!"
                    },
                    new Interaction
                    {
                        CustomerID = 2,
                        DateTime = new DateTime(1994,05,16,0,0,0),
                        TypeID = 1,
                        Description = "I have a question?"
                    },
                    new Interaction
                    {
                        CustomerID = 2,
                        DateTime = new DateTime(1996,05,16,0,0,0),
                        TypeID = 2,
                        Description = "I just bought a boat navigation system!"
                    }
                });

                context.SaveChanges();
            }
        }

        private static void CreateNews(CrmDbContext context)
        {
            if (!context.News.Any())
            {
                context.News.AddRange(new List<News>
                {
                    new News
                    {
                        Title = "5G Rollout Accelerates Globally, Paving the Way for N5G Networks",
                        Content = "The global rollout of 5G networks is accelerating, with telecom companies expanding coverage worldwide. This is setting the stage for next-generation networks like N5G, which will offer even faster speeds and enhanced connectivity for IoT devices.",
                        NewsTypeID = 2,
                    },
                    new News
                    {
                        Title = "Telecom Giants Partner to Develop N5G Networks for Smarter Cities",
                        Content = "Major telecom providers are collaborating to develop N5G (Next Generation 5G) networks designed to enable smarter cities. These networks will support advanced applications such as autonomous vehicles, smart infrastructure, and enhanced urban management.",
                        NewsTypeID = 2,
                    },
                    new News
                    {
                        Title = "Europe Leads the World in 5G Connectivity with N5G Trials Underway",
                        Content = "Europe continues to lead the world in 5G adoption, with trials of N5G networks currently underway in several countries. These trials are focused on ultra-low latency applications, including telemedicine and industrial automation, marking a significant step toward the future of mobile connectivity.",
                        NewsTypeID = 2,
                    },
                    new News
                    {
                        Title = "China's Huawei Prepares for N5G Launch as 5G Networks Expand",
                        Content = "China's telecom giant Huawei is preparing for the launch of N5G technologies, which will complement the rapid expansion of 5G networks globally. The company is focusing on enhancing network efficiency and security to address growing concerns around data privacy and cyber threats.",
                        NewsTypeID = 2,
                    },
                    new News
                    {
                        Title = "Telecom Industry Faces Challenges with 5G Spectrum Allocation for N5G Networks",
                        Content = "The telecom industry is facing challenges in spectrum allocation as governments work to ensure enough bandwidth for the continued growth of 5G networks and the upcoming N5G technologies. These challenges could impact the speed and scalability of future telecom infrastructure.",
                        NewsTypeID = 2,
                    },
                    new News
                    {
                        Title = "Company Expands to New Markets",
                        Content = "We are excited to announce our expansion into new international markets...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "New Product Launch",
                        Content = "Introducing our latest innovation, designed to make your life easier...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "Annual Report Released",
                        Content = "Our annual report highlights our achievements and future goals...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "Employee of the Month Announced",
                        Content = "Congratulations to Jane Doe for her outstanding contributions this month...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "Sustainability Initiative",
                        Content = "Our company is taking new steps to reduce its carbon footprint and promote green energy...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "Upcoming Webinar",
                        Content = "Join us for an exclusive webinar on industry trends and insights...",
                        NewsTypeID = 1,
                    },
                    new News
                    {
                        Title = "Office Renovation Update",
                        Content = "Exciting changes are coming to our headquarters to improve workspace efficiency and comfort...",
                        NewsTypeID = 1,
                    },
                });

                context.SaveChanges();
            }
        }
    }
}
