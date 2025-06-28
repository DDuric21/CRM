using Models.DTO;

namespace Models.Responses
{
    public class RetrieveNewsRS : ResponseBase
    {
        public RetrieveNewsRS(bool isSuccess, IEnumerable<NewsDTO> news = null, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            News = news;
        }

        public RetrieveNewsRS()
        {
        }

        public IEnumerable<NewsDTO> News { get; set; }
    }
}
