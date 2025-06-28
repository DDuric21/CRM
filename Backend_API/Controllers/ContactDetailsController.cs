using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace Backend_API.Controllers
{
    [Route("/ContactDetails")]
    public class ContactDetailsController : AuthorizationController
    {
        private readonly IContactDetailsService _contactDetailsService;

        public ContactDetailsController(IContactDetailsService contactDetailsService)
        {
            _contactDetailsService = contactDetailsService;
        }

        [HttpPut]
        public async Task<IActionResult> EditContactDetails([FromBody] EditContactDetailsRQ editContactDetailsRQ)
        {
            if (editContactDetailsRQ == null)
            {
                return HttpContext.BadRequest();
            }

            var result = await _contactDetailsService.EditContactDetailsAsync(editContactDetailsRQ);

            return Ok(result);
        }
    }
}
