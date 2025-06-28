using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Controllers
{
    [Route("News")]
    public class NewsController : AuthorizationController
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost]
        public async Task<IActionResult> RetrieveNews([FromBody] IEnumerable<RetrieveNewsRQ> retrieveNewsRQs)
        {
            if (retrieveNewsRQs.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var news = await _newsService.RetrieveNewsAsync(retrieveNewsRQs);

            return Ok(new RetrieveNewsRS(true, news));
        }
    }
}
