using Models.DTO;
using Models.Enums;

namespace Models.Requests
{
    public class OrderFilterRQ : RequestBase
    {
        public string OrderID { get; set; }

        public Dictionary<long, string> AssetTypes { get; set; }

        public IEnumerable<OrderStatus> OrderStatuses { get; set; }

        public DateTime? CreatedDateStart { get; set; }

        public DateTime? CreatedDateEnd { get; set; }

        public DateTime? SubmittedDateStart { get; set; }

        public DateTime? SubmittedDateEnd { get; set; }
    }
}
