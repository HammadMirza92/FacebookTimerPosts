using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class UpdatePostDto
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        [Range(1, 60)]
        public int? IntervalMinutes { get; set; }

        public string BackgroundImageUrl { get; set; }

        public string FontFamily { get; set; }

        public string PrimaryColor { get; set; }

        public bool? HasOverlay { get; set; }
    }
}
