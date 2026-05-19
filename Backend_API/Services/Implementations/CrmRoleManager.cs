using Backend_API.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
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

        public async Task<IEnumerable<Claim>> GetClaimsAsync(string roleName)
        {
            var identityRole = base.Roles
                .FirstOrDefault(x => x.Name == roleName);

            if (identityRole is null)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            var claims = await base.GetClaimsAsync(identityRole);

            return claims;
        }

        public async Task<bool> AddClaimsAsync(string roleName, IEnumerable<Claim> claims)
        {
            var identityRole = base.Roles
                .FirstOrDefault(x => x.Name == roleName);

            if (identityRole is null)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            if (Store is RoleStore<IdentityRole, CrmDbContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>> efStore)
            {
                using var transaction = await efStore.Context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var claim in claims)
                    {
                        var result = await base.AddClaimAsync(identityRole, claim);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to add claim: {string.Join(", ", claim.Value)}");
                        }
                    }
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await efStore.Context.Database.RollbackTransactionAsync();
                    throw;
                }
            }
            else
            {
                throw new InvalidOperationException("Store is not properly configured or does not support transactions.");
            }
        }

        public async Task<bool> RemoveClaimsAsync(string roleName, IEnumerable<Claim> claims)
        {
            var identityRole = base.Roles
                .FirstOrDefault(x => x.Name == roleName);

            if (identityRole is null)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            if (Store is RoleStore<IdentityRole, CrmDbContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>> efStore)
            {
                using var transaction = await efStore.Context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var claim in claims)
                    {
                        var result = await base.RemoveClaimAsync(identityRole, claim);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to remove claim: {string.Join(", ", claim.Value)}");
                        }
                    }
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await efStore.Context.Database.RollbackTransactionAsync();
                    throw;
                }
            }
            else
            {
                throw new InvalidOperationException("Store is not properly configured or does not support transactions.");
            }
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            var identityRole = await base.FindByNameAsync(roleName);

            if (identityRole is null)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            var result = await base.DeleteAsync(identityRole);

            return result.Succeeded;
        }

        public async Task<bool> CreateRoleAsync(IdentityRole role, IEnumerable<Claim> roleClaims)
        {
            var result = await base.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return await AddClaimsAsync(role.Name, roleClaims);
        }
    }
}
