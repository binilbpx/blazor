using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BalzorApp.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            // Initialize with some sample users
            _users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Password = HashPassword("admin123"),
                    Email = "admin@example.com",
                    Role = "Admin"
                },
                new User
                {
                    Username = "user",
                    Password = HashPassword("user123"),
                    Email = "user@example.com",
                    Role = "User"
                }
            };
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserAsync(username);
            if (user == null) return false;

            return user.Password == HashPassword(password);
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_users.ToList());
        }

        public async Task CreateUserAsync(string username, string password, string role)
        {
            if (_users.Any(u => u.Username == username)) return;
            _users.Add(new User
            {
                Username = username,
                Password = HashPassword(password),
                Role = role,
                Email = $"{username}@example.com"
            });
            await Task.CompletedTask;
        }

        public async Task UpdateUserAsync(string username, string password, string role)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(password))
                    user.Password = HashPassword(password);
                user.Role = role;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteUserAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                _users.Remove(user);
            }
            await Task.CompletedTask;
        }

        public async Task<ClaimsPrincipal?> GetClaimsPrincipalAsync(string username)
        {
            var user = await GetUserAsync(username);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "Custom");
            return new ClaimsPrincipal(identity);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 