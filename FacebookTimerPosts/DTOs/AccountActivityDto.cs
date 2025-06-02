namespace FacebookTimerPosts.DTOs
{
    public class AccountActivityDto
    {
        public int TotalLogins { get; set; }
        public int UniqueDevices { get; set; }
        public int ActiveSessions { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public List<LoginHistoryDto> RecentLogins { get; set; } = new();
        public List<SecurityEventDto> RecentSecurityEvents { get; set; } = new();
        public List<DeviceInfoDto> TrustedDevices { get; set; } = new();
    }
}
