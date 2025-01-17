using Microsoft.AspNetCore.Identity;

namespace Backend_API.Data.Model
{
    public class User : IdentityUser, ITrackChanges
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
