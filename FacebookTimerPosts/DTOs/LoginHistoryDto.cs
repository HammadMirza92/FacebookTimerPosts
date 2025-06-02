namespace FacebookTimerPosts.DTOs
{
    public class LoginHistoryDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
        public TimeSpan? SessionDuration { get; set; }
    }
}
