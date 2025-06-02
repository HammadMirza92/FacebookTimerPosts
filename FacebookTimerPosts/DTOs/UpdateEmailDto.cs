using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class UpdateEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
