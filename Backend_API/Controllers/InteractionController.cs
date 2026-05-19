using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Responses;

namespace Backend_API.Controllers
{
    [Route("Interactions")]
    public class InteractionController : AuthorizationController
    {
        private readonly IInteractionService _interactionService;

        public InteractionController(
            IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInteraction([FromBody] InteractionDTO interactionDTO)
        {
            if (interactionDTO.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var interactonID = await _interactionService.CreateInteractionAsync(interactionDTO);

            if (interactonID == 0)
            {
                return Problem("Interaction not created!");
            }

            return Ok(new CreateInteractionRS(interactonID));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInteraction([FromBody] InteractionDTO interactionDTO)
        {
            if (interactionDTO.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var result = await _interactionService.UpdateInteractionAsync(interactionDTO);

            if (result <= 0)
            {
                return Problem("No interaction updated");
            }

            return Ok(new ResponseBase { IsSuccess = true });
        }
    }
}
