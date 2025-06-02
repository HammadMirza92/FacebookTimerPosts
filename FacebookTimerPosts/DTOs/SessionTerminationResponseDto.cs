namespace FacebookTimerPosts.DTOs
{
    public class SessionTerminationResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int TerminatedSessionsCount { get; set; }
        public List<string> TerminatedSessionIds { get; set; } = new();
    }
}
