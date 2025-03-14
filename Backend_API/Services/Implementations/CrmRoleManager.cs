using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Backend_API.Services
{
    public class CrmRoleManager : RoleManager<IdentityRole>
    {
        public CrmRoleManager(
            IRoleStore<IdentityRole> store,
            IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<IdentityRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        { 
        }

        public async Task<Dictionary<IdentityRole, IEnumerable<Claim>>> GetClaimsAsync(IEnumerable<string> roles)
        {
            var rolesClaims = new Dictionary<IdentityRole, IEnumerable<Claim>>();

            var identityRoles = base.Roles
                .Where(x => roles.Contains(x.Name))
                .ToList();

            foreach (var identityRole in identityRoles)
            {
                var claims = await base.GetClaimsAsync(identityRole);

                rolesClaims.Add(identityRole, claims);
            }

            return rolesClaims;
        }
    }
}
