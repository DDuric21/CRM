using Backend_API.Data.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend_API.Data.DataClasses
{
    public class UserData
    {
        public User User { get; set; }

        public HashSet<string> UserRoles { get; set; }
    }
}
