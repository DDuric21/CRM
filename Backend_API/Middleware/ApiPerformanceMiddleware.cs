using Backend_API.Logging;
using System.Diagnostics;

namespace Backend_API.Middleware
{
    public class ApiPerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private const string LogFolder = "ApiPerformance";

        public ApiPerformanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var actionName = context.Request.RouteValues["action"]?.ToString();

            DynamicLogger.Log(LogFolder, $"Response time {stopwatch.ElapsedMilliseconds} ms", actionName);
        }
    }
}
