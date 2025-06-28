using Models.DTO;

namespace Models.Responses
{
    public class UpdateBillingProfileRS : ResponseBase
    {
        public BillingProfileDTO BillingProfileDTO { get; set; }

        public UpdateBillingProfileRS()
        {
        }

        public UpdateBillingProfileRS(BillingProfileDTO billingProfileDTO)
        {
            IsSuccess = true;
            BillingProfileDTO = billingProfileDTO;
        }
    }
}
