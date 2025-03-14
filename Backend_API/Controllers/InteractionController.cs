using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Responses;

namespace Backend_API.Controllers
{
    public class InteractionController : Controller
    {
        private readonly IInteractionService _interactionService;

        public InteractionController(
            IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpPost]
        [Route("/Interactions")]
        public async Task<IActionResult> CreateInteraction([FromBody] InteractionDTO interactionDTO)
        {
            if (interactionDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var interaction = _interactionService.MapDtoToInteraction(interactionDTO);
                await _interactionService.CreateInteractionAsync(interaction);

                if (interaction.Id == 0)
                {
                    return Problem("Interaction not created!");
                }

                return Ok(interaction.Id);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Route("/Interactions")]
        public async Task<IActionResult> UpdateInteraction([FromBody] InteractionDTO interactionDTO)
        {
            if (interactionDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var interaction = _interactionService.MapDtoToInteraction(interactionDTO);
                var result = await _interactionService.UpdateInteractionAsync(interaction);

                if (result <= 0)
                {
                    return Problem("No interaction updated");
                }

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }
    }
}
