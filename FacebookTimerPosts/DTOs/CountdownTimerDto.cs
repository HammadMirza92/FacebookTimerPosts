namespace FacebookTimerPosts.DTOs
{
    public class CountdownTimerDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string PublicId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PostDto Post { get; set; }
    }
}
