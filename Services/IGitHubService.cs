using BalzorApp.Models;

namespace BalzorApp.Services
{
    public interface IGitHubService
    {
        bool IsAuthenticated();
        string? GetAccessToken();
        Task<GitHubUserData?> GetUserDataAsync(string username);
        Task<List<GitHubRepository>> GetRepositoriesAsync(string username, bool includePrivate = false);
        Task<List<GitHubPullRequest>> GetPullRequestsAsync(string username, string repoName);
        Task<List<GitHubIssue>> GetIssuesAsync(string username, string repoName);
        Task<GitHubRateLimit?> GetRateLimitAsync();
        Task<string> GetAuthorizationUrlAsync();
        Task<bool> AuthenticateWithCodeAsync(string code);
        Task<CodeQualityMetrics> AnalyzeCodeQualityAsync(string username, List<GitHubRepository> repositories);
        void Logout();
        Task<List<string>> GetAvailableUsersOrgsAsync();
    }
} 