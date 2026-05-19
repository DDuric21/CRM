namespace Models.Responses
{
    public class ResponseBase : IApiResponse
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public ResponseBase()
        {
        }

        public ResponseBase(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
