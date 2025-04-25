using Serilog;
using Serilog.Context;

namespace Backend_API.Logging
{
    public static class DynamicLogger
    {
        private static IHttpContextAccessor? _httpContextAccessor;
        private const string ExceptionLogFolder = "Exceptions";
        private const string ErrorLogFolder = "Errors";

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static void LogTo(string folder, string source, string message)
        {
            // Ignore OPTIONS requests
            // TODO: Find a better solution
            if (_httpContextAccessor?.HttpContext?.Request.Method == HttpMethod.Options.Method)
            {
                return;
            }

            var filePath = GetFilePath(folder);
            var logMessage = CreateLogMessage(message);

            using (LogContext.PushProperty("LogFilePath", filePath))
            {
                var logger = Log.ForContext("SourceContext", source);
                logger.Information(logMessage);
            }
        }

        public static void LogException(Exception? ex, string source, string message)
        {
            var filePath = GetFilePath(ExceptionLogFolder);
            var logMessage = CreateLogMessage(message);

            using (LogContext.PushProperty("LogFilePath", filePath))
            {
                var logger = Log.ForContext("SourceContext", source);
                logger.Error(ex, logMessage);
            }
        }

        public static void LogError(string source, string message)
        {
            var filePath = GetFilePath(ErrorLogFolder);
            var logMessage = CreateLogMessage(message);

            using (LogContext.PushProperty("LogFilePath", filePath))
            {
                var logger = Log.ForContext("SourceContext", source);
                logger.Error(logMessage);
            }
        }

        private static string CreateLogMessage(string message)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.Items["CorrelationId"]?.ToString() ?? "N/A";
            var logMessage = $"[{correlationId}] - {message}";

            return logMessage;
        }

        private static string GetFilePath(string logFolder)
        {
            var fileName = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return $"{logFolder}/{fileName}";
        }
    }
}
