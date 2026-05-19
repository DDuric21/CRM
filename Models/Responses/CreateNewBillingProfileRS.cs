using Models.DTO;

namespace Models.Responses
{
    public class CreateNewBillingProfileRS : ResponseBase
    {
        public BillingProfileDTO BillingProfile { get; set; }

        public CreateNewBillingProfileRS()
        {
        }

        public CreateNewBillingProfileRS(BillingProfileDTO billingProfile)
        {
            IsSuccess = true;
            BillingProfile = billingProfile;
        }
    }
}
