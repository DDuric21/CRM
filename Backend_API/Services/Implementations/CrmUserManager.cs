using Backend_API.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Backend_API.Services
{
    public class CrmUserManager : UserManager<User>
    {
        public CrmUserManager(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        { }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            int suffix = 1;
            string baseUserName = $"{user.FirstName.ToLower()}.{user.LastName.ToLower()}";
            string userName = baseUserName;

            while (await base.FindByNameAsync(userName) != null)
            {
                userName = $"{baseUserName}{suffix++}";
            }

            user.UserName = userName;

            return await base.CreateAsync(user, password);
        }
    }
}
