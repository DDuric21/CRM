using Models.Enums;

namespace Models.Requests
{
    public class ChangeUserStatusRQ
    {
        public string UserName { get; set; }

        public ItemState NewUserStatus { get; set; }
    }
}
