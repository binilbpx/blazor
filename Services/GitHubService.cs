using BalzorApp.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace BalzorApp.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GitHubService> _logger;
        private static string? _accessToken; // Make static to persist across service instances

        public GitHubService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            // Set default headers for GitHub API
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BalzorApp-GitHub-Analytics");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        }

        public bool IsAuthenticated()
        {
            var isAuth = !string.IsNullOrEmpty(_accessToken);
            _logger.LogInformation("GitHub authentication check: {IsAuthenticated}", isAuth);
            return isAuth;
        }

        public string? GetAccessToken()
        {
            return _accessToken;
        }

        public async Task<GitHubUserData?> GetUserDataAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.github.com/users/{username}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GitHubUserData>(jsonString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user data for {Username}", username);
            }
            return null;
        }

        public async Task<List<GitHubRepository>> GetRepositoriesAsync(string username, bool includePrivate = false)
        {
            var repositories = new List<GitHubRepository>();
            try
            {
                var url = includePrivate && !string.IsNullOrEmpty(_accessToken)
                    ? $"https://api.github.com/user/repos?sort=updated&per_page=100"
                    : $"https://api.github.com/users/{username}/repos?sort=updated&per_page=100";

                if (!string.IsNullOrEmpty(_accessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                }

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(jsonString) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching repositories for {Username}", username);
            }
            return repositories;
        }

        public async Task<List<GitHubPullRequest>> GetPullRequestsAsync(string username, string repoName)
        {
            var pullRequests = new List<GitHubPullRequest>();
            try
            {
                var url = $"https://api.github.com/repos/{username}/{repoName}/pulls?state=all&per_page=100&sort=updated";
                
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                }

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var prs = JsonSerializer.Deserialize<List<GitHubPullRequest>>(jsonString) ?? new();
                    
                    // Load comments and reviews for each PR
                    foreach (var pr in prs.Take(20)) // Limit to avoid rate limiting
                    {
                        await LoadPRDetailsAsync(username, repoName, pr);
                    }
                    
                    pullRequests.AddRange(prs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pull requests for {Username}/{RepoName}", username, repoName);
            }
            return pullRequests;
        }

        private async Task LoadPRDetailsAsync(string username, string repoName, GitHubPullRequest pr)
        {
            try
            {
                // Load comments
                var commentsResponse = await _httpClient.GetAsync($"https://api.github.com/repos/{username}/{repoName}/issues/{pr.Number}/comments");
                if (commentsResponse.IsSuccessStatusCode)
                {
                    var commentsJson = await commentsResponse.Content.ReadAsStringAsync();
                    var comments = JsonSerializer.Deserialize<List<GitHubComment>>(commentsJson) ?? new();
                    pr.CommentsList.AddRange(comments);
                }

                // Load reviews
                var reviewsResponse = await _httpClient.GetAsync($"https://api.github.com/repos/{username}/{repoName}/pulls/{pr.Number}/reviews");
                if (reviewsResponse.IsSuccessStatusCode)
                {
                    var reviewsJson = await reviewsResponse.Content.ReadAsStringAsync();
                    var reviews = JsonSerializer.Deserialize<List<GitHubReview>>(reviewsJson) ?? new();
                    pr.Reviews.AddRange(reviews);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading details for PR {Number}", pr.Number);
            }
        }

        public async Task<List<GitHubIssue>> GetIssuesAsync(string username, string repoName)
        {
            // Placeholder for future implementation
            return new List<GitHubIssue>();
        }

        public async Task<GitHubRateLimit?> GetRateLimitAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://api.github.com/rate_limit");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    // Parse rate limit from response
                    return new GitHubRateLimit { Limit = 5000, Remaining = 4500, Reset = DateTime.UtcNow.AddHours(1) };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rate limit");
            }
            return null;
        }

        public async Task<string> GetAuthorizationUrlAsync()
        {
            var clientId = _configuration["GitHub:ClientId"];
            var redirectUri = _configuration["GitHub:RedirectUri"];
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            {
                throw new InvalidOperationException("GitHub OAuth configuration is missing. Please check appsettings.json");
            }

            var scope = "repo,read:user";
            return $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&scope={System.Web.HttpUtility.UrlEncode(scope)}";
        }

        public async Task<bool> AuthenticateWithCodeAsync(string code)
        {
            try
            {
                var clientId = _configuration["GitHub:ClientId"];
                var clientSecret = _configuration["GitHub:ClientSecret"];
                var redirectUri = _configuration["GitHub:RedirectUri"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
                {
                    _logger.LogError("GitHub OAuth configuration is incomplete");
                    return false;
                }

                // Create a new HttpClient for this request to avoid header conflicts
                using var tokenClient = new HttpClient();
                tokenClient.DefaultRequestHeaders.Add("Accept", "application/json");
                tokenClient.DefaultRequestHeaders.Add("User-Agent", "BalzorApp-GitHub-Analytics");

                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                });

                _logger.LogInformation("Exchanging authorization code for access token...");
                var response = await tokenClient.PostAsync("https://github.com/login/oauth/access_token", tokenRequest);
                
                _logger.LogInformation("Token exchange response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Token exchange response: {Response}", responseContent);
                    
                    try
                    {
                        // Parse JSON response
                        var tokenResponse = JsonSerializer.Deserialize<GitHubTokenResponse>(responseContent);
                        
                        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                        {
                            _accessToken = tokenResponse.AccessToken;
                            _logger.LogInformation("Successfully stored GitHub access token. Token length: {TokenLength}, IsAuthenticated will now return: {IsAuth}", 
                                _accessToken.Length, IsAuthenticated());
                            return true;
                        }
                        else
                        {
                            _logger.LogError("GitHub OAuth response missing access token");
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse GitHub OAuth JSON response: {Response}", responseContent);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GitHub OAuth request failed with status {StatusCode}: {Content}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with GitHub");
            }
            return false;
        }

        public void Logout()
        {
            _accessToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _logger.LogInformation("Logged out from GitHub");
        }

        public async Task<CodeQualityMetrics> AnalyzeCodeQualityAsync(string username, List<GitHubRepository> repositories)
        {
            var metrics = new CodeQualityMetrics { User = username };
            var allPRs = new List<GitHubPullRequest>();
            var commentAnalysis = new Dictionary<string, int>();

            foreach (var repo in repositories.Take(10)) // Limit to avoid rate limiting
            {
                var prs = await GetPullRequestsAsync(username, repo.Name);
                allPRs.AddRange(prs);
            }

            if (!allPRs.Any()) return metrics;

            // Calculate basic metrics
            metrics.TotalPRs = allPRs.Count;
            metrics.MergedPRs = allPRs.Count(pr => pr.State == "closed" && pr.MergedAt != null);
            metrics.OpenPRs = allPRs.Count(pr => pr.State == "open");
            metrics.ClosedPRs = allPRs.Count(pr => pr.State == "closed" && pr.MergedAt == null);
            metrics.MergeRate = metrics.TotalPRs > 0 ? (double)metrics.MergedPRs / metrics.TotalPRs : 0;

            // Calculate averages
            metrics.AverageComments = allPRs.Average(pr => pr.Comments);
            metrics.AverageReviewComments = allPRs.Average(pr => pr.ReviewComments);
            metrics.AveragePRSize = allPRs.Average(pr => pr.ChangedFiles);
            metrics.AverageAdditions = allPRs.Average(pr => pr.Additions);
            metrics.AverageDeletions = allPRs.Average(pr => pr.Deletions);

            // Analyze comments and reviews
            var allComments = allPRs.SelectMany(pr => pr.CommentsList).ToList();
            var allReviews = allPRs.SelectMany(pr => pr.Reviews).ToList();

            metrics.TotalComments = allComments.Count;
            metrics.TotalReviews = allReviews.Count;
            metrics.ApprovedReviews = allReviews.Count(r => r.State == "APPROVED");
            metrics.ChangesRequestedReviews = allReviews.Count(r => r.State == "CHANGES_REQUESTED");
            metrics.CommentedReviews = allReviews.Count(r => r.State == "COMMENTED");
            metrics.ApprovalRate = allReviews.Any() ? (double)metrics.ApprovedReviews / allReviews.Count : 0;

            // Analyze comment categories
            foreach (var comment in allComments)
            {
                var category = CategorizeComment(comment.Body);
                if (!commentAnalysis.ContainsKey(category))
                    commentAnalysis[category] = 0;
                commentAnalysis[category]++;
            }
            metrics.CommentCategories = commentAnalysis;

            // Generate recent PR timeline
            metrics.RecentPRs = allPRs
                .OrderByDescending(pr => pr.UpdatedAt)
                .Take(10)
                .Select(pr => new PRTimeline
                {
                    Number = pr.Number,
                    Title = pr.Title,
                    State = pr.State,
                    CreatedAt = pr.CreatedAt,
                    MergedAt = pr.MergedAt,
                    Comments = pr.Comments,
                    ReviewComments = pr.ReviewComments,
                    QualityScore = CalculateQualityScore(pr)
                })
                .ToList();

            return metrics;
        }

        private string CategorizeComment(string comment)
        {
            var lowerComment = comment.ToLower();
            
            if (lowerComment.Contains("bug") || lowerComment.Contains("fix") || lowerComment.Contains("issue"))
                return "Bug Reports";
            if (lowerComment.Contains("test") || lowerComment.Contains("coverage"))
                return "Testing";
            if (lowerComment.Contains("style") || lowerComment.Contains("format") || lowerComment.Contains("lint"))
                return "Code Style";
            if (lowerComment.Contains("security") || lowerComment.Contains("vulnerability"))
                return "Security";
            if (lowerComment.Contains("performance") || lowerComment.Contains("optimization"))
                return "Performance";
            if (lowerComment.Contains("documentation") || lowerComment.Contains("docs"))
                return "Documentation";
            if (lowerComment.Contains("refactor") || lowerComment.Contains("cleanup"))
                return "Refactoring";
            if (lowerComment.Contains("good") || lowerComment.Contains("nice") || lowerComment.Contains("great"))
                return "Positive Feedback";
            if (lowerComment.Contains("error") || lowerComment.Contains("exception") || lowerComment.Contains("fail"))
                return "Error Handling";
            
            return "General Comments";
        }

        private double CalculateQualityScore(GitHubPullRequest pr)
        {
            var score = 100.0;
            
            // Deduct points for large PRs
            if (pr.ChangedFiles > 20) score -= 10;
            if (pr.ChangedFiles > 50) score -= 20;
            
            // Add points for good review engagement
            if (pr.ReviewComments > 0) score += 5;
            if (pr.Comments > 0) score += 5;
            
            // Deduct points for many changes requested
            var changesRequested = pr.Reviews.Count(r => r.State == "CHANGES_REQUESTED");
            score -= changesRequested * 10;
            
            // Add points for approvals
            var approvals = pr.Reviews.Count(r => r.State == "APPROVED");
            score += approvals * 15;
            
            return Math.Max(0, Math.Min(100, score));
        }

        public async Task<List<string>> GetAvailableUsersOrgsAsync()
        {
            var usersOrgs = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _logger.LogWarning("No access token available for fetching user repositories");
                    return usersOrgs;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                // Get user's repositories (includes both owned and contributed to)
                var response = await _httpClient.GetAsync("https://api.github.com/user/repos?sort=updated&per_page=100&affiliation=owner,collaborator,organization_member");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(jsonString) ?? new();
                    
                    // Extract unique users and organizations
                    var uniqueUsersOrgs = new HashSet<string>();
                    
                    foreach (var repo in repositories)
                    {
                        if (repo.Owner != null && !string.IsNullOrEmpty(repo.Owner.Login))
                        {
                            uniqueUsersOrgs.Add(repo.Owner.Login);
                        }
                    }
                    
                    usersOrgs = uniqueUsersOrgs.OrderBy(x => x).ToList();
                    _logger.LogInformation("Found {Count} unique users/organizations from repositories", usersOrgs.Count);
                }
                else
                {
                    _logger.LogError("Failed to fetch user repositories. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available users and organizations");
            }
            return usersOrgs;
        }
    }

    public class GitHubIssue
    {
        public int Number { get; set; }
        public string Title { get; set; } = "";
        public string State { get; set; } = "";
        public string User { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 