namespace Models.DTO
{
    public class LogDetails
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string LogLevel { get; set; }

        public string Url { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
