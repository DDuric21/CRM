namespace Backend_API.Data.Models
{
    public class QueueAction : BaseModel
    {
        public string Type { get; set; }

        public string Payload { get; set; }

        public int Attempts { get; set; }

        public int StatusId { get; set; }

        public DateTime? LastAttemptedAt { get; set; }

        public string? LastError { get; set; }
    }
}
