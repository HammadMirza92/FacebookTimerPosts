namespace FacebookTimerPosts.DTOs
{
    public class DeviceInfoDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsTrusted { get; set; }
        public bool IsCurrentDevice { get; set; }
    }
}
