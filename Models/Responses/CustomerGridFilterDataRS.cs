using Models.Enums;

namespace Models.Responses
{
    public class CustomerGridFilterDataRS
    {
        public IEnumerable<CustomerType> CustomerTypes { get; set; }

        public IEnumerable<ItemState> CustomerStatuses { get; set; }
    }
}
