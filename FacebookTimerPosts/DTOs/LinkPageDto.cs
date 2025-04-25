using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class LinkPageDto
    {
        [Required]
        public string PageId { get; set; }

        [Required]
        public string AccessToken { get; set; }
    }
}
