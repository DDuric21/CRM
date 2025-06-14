using Models.Authentication;
using Models.Classes;
using Resources.Translations.API;

namespace Backend_API.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUserService _userService;

        public AuthorizationService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ValidationResult> IsUserActionPermitted(string username, string actionPermission)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception(APITranslations.InvalidUsername);
            }

            var userData = await _userService.GetUserDataByNameAsync(username);

            var isPermited = userData.UserRoles
                .SelectMany(x => x.Value)
                .Any(x => x.Type == CrmJwtClaimNames.Permission
                    && x.Value == actionPermission);

            if (!isPermited)
            {
                var errorMessage = string.Format(APITranslations.UserActionNotPermitted, username);
                throw new Exception(errorMessage);
            }

            return new ValidationResult(isPermited);
        }
    }
}
