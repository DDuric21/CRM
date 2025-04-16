using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    [Authorize]
    public class AuthorizationController : Controller
    {
    }
}
