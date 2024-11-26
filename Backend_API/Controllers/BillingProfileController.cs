using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Responses;

namespace Backend_API.Controllers
{
    public class BillingProfileController : Controller
    {
        private readonly IBillingProfileService _billingProfileService; 
        private readonly IMapper _mapper;

        public BillingProfileController(
            IBillingProfileService billingProfileService,
            IMapper mapper)
        {
            _billingProfileService = billingProfileService;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("/BillingProfiles")]
        public async Task<IActionResult> CreateNewBillingProfile([FromBody] long customerID)
        {
            if (customerID <= 0)
            {
                return BadRequest();
            }

            try
            {
                var profileID = await _billingProfileService.CreateNewBillingProfileAsync(customerID);

                if (string.IsNullOrWhiteSpace(profileID))
                {
                    return Problem("Billing profile not created!");
                }
                var newBillingProfileDTO = new BillingProfileDTO
                {
                    BillingProfileId = profileID,
                    CustomerID = customerID
                };

                return Ok(newBillingProfileDTO);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("/BillingProfiles")]
        public async Task<IActionResult> UpdateBillingProfile([FromBody] BillingProfileDTO billingProfileDTO)
        {
            if (billingProfileDTO is null)
            {
                return BadRequest();
            }

            try
            {
                var billingProfile = _mapper.Map<BillingProfile>(billingProfileDTO);
                var profileID = await _billingProfileService.UpdateBillingProfileAsync(billingProfile);

                if (profileID <= 0)
                {
                    return Problem("Billing profile not updated!");
                }

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("/BillingProfiles/{billingProfileId}")]
        public async Task<IActionResult> DeactivateBillingProfile(string billingProfileId)
        {
            if (string.IsNullOrWhiteSpace(billingProfileId))
            {
                return BadRequest();
            }

            try
            {
                var result = await _billingProfileService.DeactivateBillingProfileAsync(billingProfileId);

                if (result <= 0)
                {
                    return Problem("Billing profile not deactivated!");
                }

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
