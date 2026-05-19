using Models.DTO;

namespace Models.Responses
{
    public class GetCustomerDataRS : ResponseBase
    {
        public CustomerDTO CustomerDTO { get; set; }

        public GetCustomerDataRS()
        {
        }

        public GetCustomerDataRS(CustomerDTO customerDTO)
        {
            CustomerDTO = customerDTO;
            IsSuccess = true;
        }
    }
}
