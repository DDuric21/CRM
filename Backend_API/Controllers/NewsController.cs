using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;
using Models.Requests;

namespace Backend_API.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost]
        [Route("/News")]
        public async Task<IActionResult> GetNews([FromBody] IEnumerable<RetrieveNewsRQ> retrieveNewsRQs)
        {
            if (retrieveNewsRQs.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var news = await _newsService.GetNewsAsync(retrieveNewsRQs);

                if (news.IsNullOrEmpty())
                {
                    return Problem("No news found!");
                }

                var newsDTO = _newsService.MapNewsToDTOs(news);

                return Ok(newsDTO);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

    }
}
