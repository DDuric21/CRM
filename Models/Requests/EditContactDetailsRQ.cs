using Models.DTO;

namespace Models.Requests
{
    public class EditContactDetailsRQ : RequestBase
    {
        public long CustomerId { get; set; }

        public CustomerContactDetails CustomerContactDetails { get; set; }
    }
}
