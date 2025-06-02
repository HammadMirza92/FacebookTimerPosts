namespace FacebookTimerPosts.DTOs
{
    public class AccountStatsDto
    {
        public int LoginCount { get; set; }
        public int SecurityScore { get; set; }
        public long DataUsage { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public int AccountAge { get; set; }
        public int LastLoginDays { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int ProfileCompleteness { get; set; }
    }
}
