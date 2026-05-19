using Models.Classes;

namespace Backend_API.Services
{
    public interface IAuthorizationService
    {
        Task<ValidationResult> IsUserActionPermitted(string username, string actionPermission);
    }
}
