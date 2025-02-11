using Models.Enums;

namespace Models.Requests
{
    public class CustomerFilterRQ
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PersonalID { get; set; }

        public IEnumerable<CustomerType> CustomerTypes { get; set; }

        public IEnumerable<ItemState> CustomerStatuses { get; set; }

        public DateTime? BirthdayDateStart { get; set; }

        public DateTime? BirthdayDateEnd { get; set; }
    }
}
