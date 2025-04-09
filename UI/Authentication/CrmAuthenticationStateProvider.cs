using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Models.Authentication;
using Models.Helpers;
using System.Security.Claims;
using UI.Services;

namespace UI.Authentication
{
    public class CrmAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AUTH_TYPE = "token";
        private const string USER_SESSION = "UserSession";
        private readonly ISessionStorageService _sessionStorage;
        private readonly IAuthenticationService _authenticationService;
        private ClaimsPrincipal _anonymus = new ClaimsPrincipal(new ClaimsIdentity());

        public CrmAuthenticationStateProvider(
            ISessionStorageService sessionStorage,
            IAuthenticationService authenticationService)
        {
            _sessionStorage = sessionStorage;
            _authenticationService = authenticationService;
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

                var claims = GetUserSessionClaims(userSession);

                var identity = new ClaimsIdentity(AUTH_TYPE);
                identity.AddClaims(claims);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(_anonymus));
            }
        }

        public async Task UpdateAuthenticationState(string token = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                await ClearSession();

                return;
            }

            JasonWebToken.Value = token;

            var userSession = _authenticationService.CreateUserSession();
            if (userSession.IsNullOrEmpty())
            {
                await ClearSession();

                return;
            }

            await SaveSession(userSession);
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            if (userSession.IsNullOrEmpty())
            {
                await _sessionStorage.RemoveItemAsync(USER_SESSION);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymus)));

                return;
            }

            await _sessionStorage.SaveItemEncryptedAsync(USER_SESSION, userSession);

            var claims = GetUserSessionClaims(userSession);

            var identity = new ClaimsIdentity(AUTH_TYPE);
            identity.AddClaims(claims);

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private async Task SaveSession(UserSession userSession)
        {
            await _sessionStorage.SaveItemEncryptedAsync(USER_SESSION, userSession);

            var claims = GetUserSessionClaims(userSession);

            var identity = new ClaimsIdentity(AUTH_TYPE);
            identity.AddClaims(claims);

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private async Task ClearSession()
        {
            await _sessionStorage.RemoveItemAsync(USER_SESSION);

            JasonWebToken.Value = null;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymus)));
        }

        private IEnumerable<Claim> GetUserSessionClaims(UserSession userSession)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, userSession.UserName));

            foreach (var role in userSession.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in userSession.Permissions)
            {
                claims.Add(new Claim(CrmClaimTypes.Permission, permission));
            }

            return claims;
        }
    }
}
