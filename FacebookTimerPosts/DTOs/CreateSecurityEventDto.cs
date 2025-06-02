using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class CreateSecurityEventDto
    {
        [Required]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public string? AdditionalData { get; set; }
    }
}
