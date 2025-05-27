using Models.Responses;

namespace UI.Helpers
{
    public class ActionResult<T> : IApiResponse where T : class
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public T? Data { get; set; }

        public ActionResult(bool isSuccess, string errorMessage, T? data = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Data = data;
        }

        public ActionResult(string errorMessage, T? data = null)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            Data = data;
        }

        public ActionResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public ActionResult(T data)
        {
            IsSuccess = true;
            Data = data;
        }
    }
}
