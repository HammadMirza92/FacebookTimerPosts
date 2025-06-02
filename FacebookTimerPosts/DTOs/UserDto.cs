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
        public string? PhoneNumber { get; set; }
        public string? PhotoURL { get; set; }
        public bool? EmailConfirmed { get; set; }
        public UserSubscriptionDto CurrentSubscription { get; set; }
    }
}
