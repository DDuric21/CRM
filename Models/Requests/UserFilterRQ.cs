using Models.Enums;

namespace Models.Requests
{
    public class UserFilterRQ
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<ItemState> UserStatuses { get; set; }

        public IEnumerable<string> UserRoles { get; set; }

        public DateTime? CreatedDateStart { get; set; }

        public DateTime? CreatedDateEnd { get; set; }
    }
}
