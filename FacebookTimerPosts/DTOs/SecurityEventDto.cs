namespace FacebookTimerPosts.DTOs
{
    public class SecurityEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
        public string? AdditionalData { get; set; }
    }
}
