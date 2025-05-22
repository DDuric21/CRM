namespace Models.Responses
{
    public class ResponseBase
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
