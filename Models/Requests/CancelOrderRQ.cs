namespace Models.Requests
{
    public class CancelOrderRQ : RequestBase
    {
        public Guid OrderId { get; set; }
    }
}
