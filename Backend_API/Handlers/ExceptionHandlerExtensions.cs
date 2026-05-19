using Backend_API.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models.Authentication.DataStructures;
using System.Text;

namespace Backend_API.Handlers
{
    public static class ExceptionHandlerExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionFeature?.Error;
                    var request = context.Request;

                    request.EnableBuffering();
                    string requestBody = string.Empty;

                    if (request.ContentLength > 0 && request.Body.CanSeek)
                    {
                        request.Body.Seek(0, SeekOrigin.Begin);
                        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                        requestBody = await reader.ReadToEndAsync();
                        request.Body.Seek(0, SeekOrigin.Begin);
                    }

                    var headers = request.Headers.Select(h => $"{h.Key}: {h.Value}");

                    var logMessage = new StringBuilder();
                    logMessage.AppendLine($"Unhandled Exception: {exception?.Message} ");
                    logMessage.AppendLine($"Method: {request.Method} ");
                    logMessage.AppendLine($"Path: {request.Path} ");
                    logMessage.AppendLine($"Headers: {string.Join(Environment.NewLine, headers)} ");
                    logMessage.AppendLine($"Body: {requestBody} ");
                    logMessage.AppendLine($"Exception Message: {exception?.Message}");
                    logMessage.AppendLine($"Stack Trace: {exception?.StackTrace} ");

                    DynamicLogger.LogCritical(logMessage.ToString(), nameof(ExceptionHandlerExtensions));

                    var problem = new ProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Detail = exception?.Message ?? "Unhandled Exception",
                        Status = StatusCodes.Status500InternalServerError,
                        Instance = context.Request.Path
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.Headers.Add(HttpHeaderNames.CorrelationID, context.Request.Headers[HttpHeaderNames.CorrelationID]);

                    await context.Response.WriteAsJsonAsync(problem);
                });
            });
        }
    }
}
