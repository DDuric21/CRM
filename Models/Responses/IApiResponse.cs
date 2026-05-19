namespace Models.Responses
{
    public interface IApiResponse
    {
        /// <summary>
        /// Indicates whether the API call was successful.
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        /// Contains an error message if the API call was not successful.
        /// </summary>
        string? ErrorMessage { get; set; }
    }
}
