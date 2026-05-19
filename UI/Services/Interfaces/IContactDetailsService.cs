using Models.DTO;
using UI.Helpers;

namespace UI.Services
{
    public interface IContactDetailsService
    {
        Task<ActionResult<object>> UpdateContactDetailsAsync(long customerId, CustomerContactDetails customerContactDetails);
    }
}
