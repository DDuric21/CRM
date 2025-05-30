using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Helpers
{
    public static class HttpContextExtensions
    {
        public static IActionResult BadRequest(
            this HttpContext httpContext, 
            string detail = "Invalid request body", 
            string? title = "Bad Request")
        {
            var problem = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            };

            return new BadRequestObjectResult(problem);
        }
    }
}
