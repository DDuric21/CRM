using Microsoft.AspNetCore.Mvc;
using Models.Authentication;

namespace CRM_UI.Services
{
    public interface IAuthenticationService
    {
        ActionResult<AuthenticationResult> Login(string username, string password);
    }
}
