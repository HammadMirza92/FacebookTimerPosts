using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class UpdateProfileDto
    {
        [StringLength(50, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }

        public string? Website { get; set; }

        public string? Location { get; set; }
    }
}
