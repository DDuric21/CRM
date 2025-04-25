using Backend_API.Logging;
using System.Text;

namespace Backend_API.Middleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private const string LogFolder = "ApiLogging";

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering(); 

            var requestBody = await ReadRequestBodyAsync(context);
            var requestLog = $"{context.Request.Method} URL: '...{context.Request.Path}' Body: {requestBody}";

            var originalBodyStream = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            var responseBodyText = await ReadResponseBodyAsync(memStream);

            var responseLog = $"Status: {context.Response.StatusCode} Body: {responseBodyText}";
            var logMessage = $"Request: {requestLog} {Environment.NewLine} Response: {responseLog}";
            DynamicLogger.LogTo(LogFolder, nameof(ApiLoggingMiddleware), logMessage);

            await memStream.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream memStream)
        {
            memStream.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(memStream).ReadToEndAsync();
            memStream.Seek(0, SeekOrigin.Begin);

            return responseBodyText;
        }
    }
}
