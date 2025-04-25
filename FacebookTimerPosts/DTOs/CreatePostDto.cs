using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class CreatePostDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        [Required]
        [Range(1, 60)]
        public int IntervalMinutes { get; set; } = 15;

        public string BackgroundImageUrl { get; set; }

        [Required]
        public string FontFamily { get; set; } = "Anton";

        [Required]
        public string PrimaryColor { get; set; } = "#FFFFFF";

        public bool HasOverlay { get; set; } = true;

        [Required]
        public int FacebookPageId { get; set; }

        public int? TemplateId { get; set; }
    }
}
