using Microsoft.AspNetCore.Identity;

namespace Backend_API.Data.Models
{
    public class User : IdentityUser, ITrackChanges
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int UserStatusID { get; set; }
    }
}
