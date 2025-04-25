namespace FacebookTimerPosts.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int DaysUntilExpiration { get; set; }
        public int LinkedPagesCount { get; set; }
        public int PostsRemainingToday { get; set; }
    }
}
