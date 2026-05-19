using Backend_API.Helpers;
using Backend_API.Logging;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Responses;
using Serilog.Events;
using System.Text;

namespace Backend_API.Controllers
{
    [Route("Logging")]
    public class LoggingController : ControllerBase
    {
        [HttpPost("Error")]
        public IActionResult LogError([FromBody] LogDetails logDetails)
        {
            if (logDetails.IsNullOrEmpty())
            {
                return HttpContext.BadRequest("Log details cannot be null or empty.");
            }

            if (!Enum.TryParse<LogEventLevel>(logDetails.LogLevel, true, out var logLevel))
            {
                return HttpContext.BadRequest("Invalid log level specified.");
            }

            try
            {
                var logMessage = new StringBuilder();
                logMessage.AppendLine($"Error occurred on URL: {logDetails.Url}");
                logMessage.AppendLine($"Message: {logDetails.Message}");
                logMessage.AppendLine($"StackTrace: {logDetails.StackTrace}");

                DynamicLogger.LogUIError(logLevel, logMessage.ToString());

                return Ok(new ResponseBase { IsSuccess = true });
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "An error occurred while logging the exception.");
                return Problem(ex.Message);
            }
        }       
    }
}
