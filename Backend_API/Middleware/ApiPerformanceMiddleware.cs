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

            DynamicLogger.LogTo(LogFolder, nameof(ApiPerformanceMiddleware), $"Response time {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
