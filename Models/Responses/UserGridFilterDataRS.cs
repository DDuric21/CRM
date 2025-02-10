using Models.Enums;

namespace Models.Responses
{
    public class UserGridFilterDataRS
    {
        public IEnumerable<ItemState> UserStatuses { get; set; }

        public IEnumerable<string> UserRoles { get; set; }
    }
}
