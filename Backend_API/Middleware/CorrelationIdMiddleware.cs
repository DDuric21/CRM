using Models.Authentication.DataStructures;

namespace Backend_API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            SetCorrelationID(context);

            await _next(context);
        }

        private static void SetCorrelationID(HttpContext context)
        {
            var correlationId = context.Request.Headers.ContainsKey(HttpHeaderNames.CorrelationID)
                ? context.Request.Headers[HttpHeaderNames.CorrelationID].ToString()
                : Guid.NewGuid().ToString();

            context.Items[HttpHeaderNames.CorrelationID] = correlationId;

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(HttpHeaderNames.CorrelationID))
                {
                    context.Response.Headers[HttpHeaderNames.CorrelationID] = correlationId;
                }
                return Task.CompletedTask;
            });
        }
    }
}
