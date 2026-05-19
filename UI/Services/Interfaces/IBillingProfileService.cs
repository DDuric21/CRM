using Models.DTO;
using UI.Helpers;

namespace UI.Services
{
    public interface IBillingProfileService
    {
        Task<ActionResult<BillingProfileDTO>> CreateNewBillingProfileAsync(long customerID);

        Task<ActionResult<BillingProfileDTO>> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO);

        Task<ActionResult<object>> DeactivateBillingProfileAsync(string billingProfileId);

        Task<List<InvoiceRow>> GetBillingProfileInvoicesAsync(long customerId);

        Task<SignedLinkDto> GetPdfLinkAsync(long billId, long customerId);
    }
}
