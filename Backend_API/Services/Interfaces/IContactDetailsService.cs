using Models.Requests;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IContactDetailsService
    {
        /// <summary>
        /// Asynchronously edits the contact details.
        /// </summary>
        /// <param name="editContactDetailsRQ">The request containing the contact details to edit.</param>
        /// <returns>A task that represents the asynchronous operation, containing a response indicating success or failure.</returns>
        Task<ResponseBase> EditContactDetailsAsync(EditContactDetailsRQ editContactDetailsRQ);
    }
}
