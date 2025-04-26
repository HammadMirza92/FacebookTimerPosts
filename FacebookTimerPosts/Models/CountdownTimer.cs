namespace FacebookTimerPosts.Models
{
    public class CountdownTimer
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string PublicId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Post Post { get; set; }

        public CountdownTimer()
        {
            PublicId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
