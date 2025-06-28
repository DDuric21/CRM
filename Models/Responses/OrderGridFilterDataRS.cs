using Models.Enums;

namespace Models.Responses
{
    public class OrderGridFilterDataRS : ResponseBase
    {
        public IEnumerable<OrderStatus> OrderStatuses { get; set; }

        public Dictionary<long, string> AssetTypes { get; set; }
    }
}
