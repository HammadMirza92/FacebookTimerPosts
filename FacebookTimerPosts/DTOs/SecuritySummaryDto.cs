namespace FacebookTimerPosts.DTOs
{
    public class SecuritySummaryDto
    {
        public int SecurityScore { get; set; }
        public int TotalLogins { get; set; }
        public int FailedLoginAttempts { get; set; }
        public int ActiveSessions { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public DateTime? LastSuccessfulLogin { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int SecurityEventsLast30Days { get; set; }
        public List<string> SecurityRecommendations { get; set; } = new();
    }
}
