using System.Text.Json;
using System.Text.Json.Serialization;

namespace BalzorApp.Models
{
    public class GitHubUserData
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = "";
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
        
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = "";
        
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }
        
        [JsonPropertyName("public_repos")]
        public int PublicRepos { get; set; }
        
        [JsonPropertyName("followers")]
        public int Followers { get; set; }
        
        [JsonPropertyName("following")]
        public int Following { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";
        
        [JsonPropertyName("company")]
        public string? Company { get; set; }
        
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [JsonPropertyName("blog")]
        public string? Blog { get; set; }
        
        [JsonPropertyName("twitter_username")]
        public string? TwitterUsername { get; set; }
        
        [JsonPropertyName("hireable")]
        public bool? Hireable { get; set; }
        
        [JsonPropertyName("public_gists")]
        public int PublicGists { get; set; }
        
        [JsonPropertyName("site_admin")]
        public bool SiteAdmin { get; set; }
    }

    public class GitHubRepository
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = "";
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("language")]
        public string? Language { get; set; }
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
        
        [JsonPropertyName("fork")]
        public bool Fork { get; set; }
        
        [JsonPropertyName("archived")]
        public bool Archived { get; set; }
        
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
        
        [JsonPropertyName("has_issues")]
        public bool HasIssues { get; set; }
        
        [JsonPropertyName("has_projects")]
        public bool HasProjects { get; set; }
        
        [JsonPropertyName("has_downloads")]
        public bool HasDownloads { get; set; }
        
        [JsonPropertyName("has_wiki")]
        public bool HasWiki { get; set; }
        
        [JsonPropertyName("has_pages")]
        public bool HasPages { get; set; }
        
        [JsonPropertyName("has_discussions")]
        public bool HasDiscussions { get; set; }
        
        [JsonPropertyName("stargazers_count")]
        public int StargazersCount { get; set; }
        
        [JsonPropertyName("watchers_count")]
        public int WatchersCount { get; set; }
        
        [JsonPropertyName("forks_count")]
        public int ForksCount { get; set; }
        
        [JsonPropertyName("open_issues_count")]
        public int OpenIssuesCount { get; set; }
        
        [JsonPropertyName("size")]
        public int Size { get; set; }
        
        [JsonPropertyName("default_branch")]
        public string DefaultBranch { get; set; } = "";
        
        [JsonPropertyName("visibility")]
        public string Visibility { get; set; } = "";
        
        [JsonPropertyName("private")]
        public bool Private { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        
        [JsonPropertyName("pushed_at")]
        public DateTime? PushedAt { get; set; }
        
        [JsonPropertyName("license")]
        public GitHubLicense? License { get; set; }
        
        [JsonPropertyName("owner")]
        public GitHubUserData? Owner { get; set; }
        
        [JsonPropertyName("permissions")]
        public GitHubPermissions? Permissions { get; set; }
    }

    public class GitHubLicense
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = "";
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class GitHubPermissions
    {
        [JsonPropertyName("admin")]
        public bool Admin { get; set; }
        
        [JsonPropertyName("maintain")]
        public bool Maintain { get; set; }
        
        [JsonPropertyName("push")]
        public bool Push { get; set; }
        
        [JsonPropertyName("triage")]
        public bool Triage { get; set; }
        
        [JsonPropertyName("pull")]
        public bool Pull { get; set; }
    }

    public class GitHubPullRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("number")]
        public int Number { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";
        
        [JsonPropertyName("state")]
        public string State { get; set; } = "";
        
        [JsonPropertyName("user")]
        public GitHubUserData? User { get; set; }
        
        [JsonPropertyName("merged_at")]
        public DateTime? MergedAt { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("closed_at")]
        public DateTime? ClosedAt { get; set; }
        
        [JsonPropertyName("comments")]
        public int Comments { get; set; }
        
        [JsonPropertyName("review_comments")]
        public int ReviewComments { get; set; }
        
        [JsonPropertyName("commits")]
        public int Commits { get; set; }
        
        [JsonPropertyName("additions")]
        public int Additions { get; set; }
        
        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }
        
        [JsonPropertyName("changed_files")]
        public int ChangedFiles { get; set; }
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
        
        [JsonPropertyName("diff_url")]
        public string DiffUrl { get; set; } = "";
        
        [JsonPropertyName("patch_url")]
        public string PatchUrl { get; set; } = "";
        
        [JsonPropertyName("issue_url")]
        public string IssueUrl { get; set; } = "";
        
        [JsonPropertyName("assignees")]
        public List<GitHubUserData> Assignees { get; set; } = new();
        
        [JsonPropertyName("requested_reviewers")]
        public List<GitHubUserData> RequestedReviewers { get; set; } = new();
        
        [JsonPropertyName("labels")]
        public List<GitHubLabel> Labels { get; set; } = new();
        
        [JsonPropertyName("head")]
        public GitHubBranch Head { get; set; } = new();
        
        [JsonPropertyName("base")]
        public GitHubBranch Base { get; set; } = new();
        
        [JsonPropertyName("draft")]
        public bool Draft { get; set; }
        
        [JsonPropertyName("merged")]
        public bool Merged { get; set; }
        
        [JsonPropertyName("mergeable")]
        public bool? Mergeable { get; set; }
        
        [JsonPropertyName("mergeable_state")]
        public string MergeableState { get; set; } = "";
        
        [JsonPropertyName("merged_by")]
        public GitHubUserData? MergedBy { get; set; }
        
        [JsonPropertyName("merge_commit_sha")]
        public string? MergeCommitSha { get; set; }
        
        [JsonPropertyName("body")]
        public string? Body { get; set; }
        
        // Custom properties for our analysis
        public List<GitHubComment> CommentsList { get; set; } = new();
        public List<GitHubReview> Reviews { get; set; } = new();
    }

    public class GitHubBranch
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = "";
        
        [JsonPropertyName("ref")]
        public string Ref { get; set; } = "";
        
        [JsonPropertyName("sha")]
        public string Sha { get; set; } = "";
        
        [JsonPropertyName("user")]
        public GitHubUserData? User { get; set; }
        
        [JsonPropertyName("repo")]
        public GitHubRepository? Repo { get; set; }
    }

    public class GitHubLabel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonPropertyName("color")]
        public string Color { get; set; } = "";
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class GitHubComment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("user")]
        public GitHubUserData? User { get; set; }
        
        [JsonPropertyName("body")]
        public string Body { get; set; } = "";
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
        
        [JsonPropertyName("issue_url")]
        public string IssueUrl { get; set; } = "";
        
        [JsonPropertyName("author_association")]
        public string AuthorAssociation { get; set; } = "";
        
        // Custom properties for our analysis
        public string Type { get; set; } = "comment"; // "comment", "review", "review_comment"
        public string? ReviewState { get; set; } // "APPROVED", "CHANGES_REQUESTED", "COMMENTED"
    }

    public class GitHubReview
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("user")]
        public GitHubUserData? User { get; set; }
        
        [JsonPropertyName("state")]
        public string State { get; set; } = ""; // "APPROVED", "CHANGES_REQUESTED", "COMMENTED", "DISMISSED"
        
        [JsonPropertyName("body")]
        public string? Body { get; set; }
        
        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }
        
        [JsonPropertyName("commit_id")]
        public string CommitId { get; set; } = "";
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";
        
        [JsonPropertyName("pull_request_url")]
        public string PullRequestUrl { get; set; } = "";
        
        [JsonPropertyName("author_association")]
        public string AuthorAssociation { get; set; } = "";
        
        [JsonPropertyName("comments_url")]
        public string CommentsUrl { get; set; } = "";
        
        [JsonPropertyName("_links")]
        public GitHubLinks? Links { get; set; }
        
        // Custom properties for our analysis
        public List<GitHubComment> Comments { get; set; } = new();
    }

    public class GitHubLinks
    {
        [JsonPropertyName("html")]
        public GitHubLink? Html { get; set; }
        
        [JsonPropertyName("pull_request")]
        public GitHubLink? PullRequest { get; set; }
    }

    public class GitHubLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = "";
    }

    public class GitHubRateLimit
    {
        public int Limit { get; set; }
        public int Remaining { get; set; }
        public DateTime Reset { get; set; }
    }

    public class CodeQualityMetrics
    {
        public string User { get; set; } = "";
        public int TotalPRs { get; set; }
        public int MergedPRs { get; set; }
        public int OpenPRs { get; set; }
        public int ClosedPRs { get; set; }
        public double MergeRate { get; set; }
        public double AverageComments { get; set; }
        public double AverageReviewComments { get; set; }
        public double AveragePRSize { get; set; }
        public double AverageAdditions { get; set; }
        public double AverageDeletions { get; set; }
        public int TotalComments { get; set; }
        public int TotalReviews { get; set; }
        public int ApprovedReviews { get; set; }
        public int ChangesRequestedReviews { get; set; }
        public int CommentedReviews { get; set; }
        public double ApprovalRate { get; set; }
        public List<string> CommonIssues { get; set; } = new();
        public Dictionary<string, int> CommentCategories { get; set; } = new();
        public List<PRTimeline> RecentPRs { get; set; } = new();
    }

    public class PRTimeline
    {
        public int Number { get; set; }
        public string Title { get; set; } = "";
        public string State { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? MergedAt { get; set; }
        public int Comments { get; set; }
        public int ReviewComments { get; set; }
        public double QualityScore { get; set; }
    }

    public class CommentAnalysis
    {
        public string Category { get; set; } = "";
        public int Count { get; set; }
        public double Percentage { get; set; }
        public List<string> Examples { get; set; } = new();
    }

    public class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";
        
        [JsonPropertyName("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = "";
    }
} 