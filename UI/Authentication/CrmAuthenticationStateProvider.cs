using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Models.HelperMethods;
using System.Security.Claims;

namespace UI.Authentication
{
    public class CrmAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AUTH_TYPE = "token";
        private const string USER_SESSION = "UserSession";
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _anonymus = new ClaimsPrincipal(new ClaimsIdentity());

        public CrmAuthenticationStateProvider(
            ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorage.ReadEncryptedItemAsync<UserSession>(USER_SESSION);

                if (userSession.IsNullOrEmpty())
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

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            if (userSession.IsNullOrEmpty())
            {
                await _sessionStorage.RemoveItemAsync(USER_SESSION);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymus)));
            }
            else
            {
                await _sessionStorage.SaveItemEncryptedAsync(USER_SESSION, userSession);

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
