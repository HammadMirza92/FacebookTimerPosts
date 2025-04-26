namespace FacebookTimerPosts.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public UserSubscriptionDto CurrentSubscription { get; set; }
    }
}
