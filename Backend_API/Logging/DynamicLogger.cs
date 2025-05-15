using Serilog.Context;
using Serilog.Events;
using System.Runtime.CompilerServices;

namespace Backend_API.Logging
{
    public static class DynamicLogger
    {
        private static IHttpContextAccessor? _httpContextAccessor;
        private const string ExceptionLogFolder = "Exceptions";
        private const string ErrorLogFolder = "Errors";
        private const string WarningLogFolder = "HME";
        private const string CriticalErrorsLogFolder = "CRE";

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static void Log(string folder, string message, [CallerMemberName] string source = "")
        {
            LogTo(folder, message, source, LogEventLevel.Information);
        }

        public static void LogError(string message, [CallerMemberName] string source = "")
        {
            LogTo(ErrorLogFolder, message, source, LogEventLevel.Error);
        }

        public static void LogWarning(string message, [CallerMemberName] string source = "")
        {
            LogTo(WarningLogFolder, message, source, LogEventLevel.Warning);
        }

        public static void LogException(Exception? ex, string message, [CallerMemberName] string source = "")
        {
            var filePath = GetFilePath(ExceptionLogFolder);
            var logMessage = CreateLogMessage(message);

            using (LogContext.PushProperty("LogFilePath", filePath))
            {
                var logger = Serilog.Log.ForContext("SourceContext", source);
                logger.Error(ex, logMessage);
            }
        }

        public static void LogCritical(string message, [CallerMemberName] string source = "")
        {
            LogTo(CriticalErrorsLogFolder, message, source, LogEventLevel.Fatal);
        }

        private static void LogTo(string folder, string message, string source, LogEventLevel logEventLevel)
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
                var logger = Serilog.Log.ForContext("SourceContext", source);
                logger.Write(logEventLevel, logMessage);
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
