using Backend_API.Data.Model;

namespace Backend_API.Data.DataClasses
{
    public class UserData
    {
        public User User { get; set; }

        public IEnumerable<string> UserRoles { get; set; }
    }
}
