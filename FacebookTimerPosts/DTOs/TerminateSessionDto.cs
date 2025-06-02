using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class TerminateSessionDto
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
