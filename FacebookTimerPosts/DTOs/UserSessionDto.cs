namespace FacebookTimerPosts.DTOs
{
    public class UserSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsCurrent { get; set; }
    }
}
