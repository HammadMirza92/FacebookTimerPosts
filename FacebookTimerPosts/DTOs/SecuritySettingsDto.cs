namespace FacebookTimerPosts.DTOs
{
    public class SecuritySettingsDto
    {
        public bool TwoFactorEnabled { get; set; }
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool LoginAlerts { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public List<UserLoginInfoDto> ExternalLogins { get; set; } = new();
        public List<UserSessionDto> ActiveSessions { get; set; } = new();
        public List<SecurityEventDto> SecurityEvents { get; set; } = new();
    }
}
