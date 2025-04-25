using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class PublishPostDto
    {
        [Required]
        public string BaseUrl { get; set; }
    }
}
