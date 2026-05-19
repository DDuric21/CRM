using Models.Enums;

namespace Models.Requests
{
    public class RetrieveNewsRQ
    {
        public int Amount { get; set; }

        public NewsType NewsType { get; set; }
    }
}
