using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BalzorApp.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IUserService _userService;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IUserService userService)
        {
            _userService = userService;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public async Task LoginAsync(string username, string password)
        {
            var isValid = await _userService.ValidateUserAsync(username, password);
            if (isValid)
            {
                var claimsPrincipal = await _userService.GetClaimsPrincipalAsync(username);
                if (claimsPrincipal != null)
                {
                    _currentUser = claimsPrincipal;
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
                }
            }
        }

        public void Logout()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
} 