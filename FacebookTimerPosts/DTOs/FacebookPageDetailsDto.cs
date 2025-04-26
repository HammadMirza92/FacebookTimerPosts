namespace FacebookTimerPosts.DTOs
{
    public class FacebookPageDetailsDto
    {
        public int Id { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
        public DateTime TokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
