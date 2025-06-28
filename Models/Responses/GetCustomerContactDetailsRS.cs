using Models.DTO;

namespace Models.Responses
{
    public class GetCustomerContactDetailsRS : ResponseBase
    {
        public CustomerContactDetails CustomerContactDetails { get; set; }

        public GetCustomerContactDetailsRS()
        {
        }

        public GetCustomerContactDetailsRS(CustomerContactDetails customerContactDetails)
        {
            CustomerContactDetails = customerContactDetails;
            IsSuccess = true;
        }
    }
}
