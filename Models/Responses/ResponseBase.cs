namespace Models.Responses
{
    public class ResponseBase : IApiResponse
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
