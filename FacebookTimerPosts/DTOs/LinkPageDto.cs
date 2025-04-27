namespace FacebookTimerPosts.DTOs
{
    public class LinkPageDto
    {
        public string Id { get; set; }
        public string PageId { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string Category { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
