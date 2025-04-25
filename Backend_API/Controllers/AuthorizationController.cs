using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    [Authorize]
    public abstract class AuthorizationController : Controller
    {
    }
}
