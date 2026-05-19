using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Resources.Translations.API;

namespace Backend_API.Controllers
{
    [Route("BillingProfiles")]
    public class BillingProfileController : AuthorizationController
    {
        private readonly IBillingProfileService _billingProfileService; 


        public BillingProfileController(IBillingProfileService billingProfileService)
        {
            _billingProfileService = billingProfileService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBillingProfile([FromBody] long customerID)
        {
            if (customerID <= 0)
            {
                return HttpContext.BadRequest(APITranslations.InvalidCustomerIdProvided);
            }

            var profile = await _billingProfileService.CreateNewBillingProfileAsync(customerID);

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBillingProfile([FromBody] BillingProfileDTO billingProfileDTO)
        {
            if (billingProfileDTO is null)
            {
                return HttpContext.BadRequest();
            }

            var response = await _billingProfileService.UpdateBillingProfileAsync(billingProfileDTO);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{billingProfileId}")]
        public async Task<IActionResult> DeactivateBillingProfile(string billingProfileId)
        {
            if (string.IsNullOrWhiteSpace(billingProfileId))
            {
                return HttpContext.BadRequest(APITranslations.InvalidBillingProfileID);
            }

            var result = await _billingProfileService.DeactivateBillingProfileAsync(billingProfileId);

            return Ok(result);
        }
    }
}
