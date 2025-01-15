using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class User : IdentityUser, ITrackChanges
    {
        public string FristName { get; set; }

        public string LastName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
