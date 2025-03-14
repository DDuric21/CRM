using Backend_API.Data.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Backend_API.Data.DataClasses
{
    public class UserData
    {
        public User User { get; set; }

        public Dictionary<IdentityRole, IEnumerable<Claim>> UserRoles { get; set; }
    }
}
