using System.Security.Claims;

namespace BalzorApp.Services
{
    public interface IUserService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<User?> GetUserAsync(string username);
        Task<ClaimsPrincipal?> GetClaimsPrincipalAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(string username, string password, string role);
        Task UpdateUserAsync(string username, string password, string role);
        Task DeleteUserAsync(string username);
    }
} 