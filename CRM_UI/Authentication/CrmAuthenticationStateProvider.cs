using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Models.HelperMethods;
using System.Security.Claims;

namespace CRM_UI.Authentication
{
    public class CrmAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AUTH_TYPE = "token";
        private const string USER_SESSION = "UserSession";
        private readonly ProtectedSessionStorage _protectedSessionStorage;
        private ClaimsPrincipal _anonymus = new ClaimsPrincipal(new ClaimsIdentity());

        public CrmAuthenticationStateProvider(
            ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResoult = await _protectedSessionStorage.GetAsync<UserSession>(USER_SESSION);
                var userSession = userSessionStorageResoult.Success
                    ? userSessionStorageResoult.Value
                    : null;

                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymus));
                }

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    //new Claim(ClaimTypes.Role, userSession.Role),
                };

                var identity = new ClaimsIdentity(AUTH_TYPE);
                identity.AddClaims(claims);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymus));
            }
        }

        public async void UpdateAuthenticationState(UserSession userSession)
        {
            if (userSession.IsNullOrEmpty())
            {
                await _protectedSessionStorage.DeleteAsync(USER_SESSION);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymus)));
            }
            else
            {
                await _protectedSessionStorage.SetAsync(USER_SESSION, userSession);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    //new Claim(ClaimTypes.Role, userSession.Role),
                };

                var identity = new ClaimsIdentity(AUTH_TYPE);
                identity.AddClaims(claims);
                var user = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
        }
    }
}
