namespace FacebookTimerPosts.DTOs
{
    public class ExternalAuthResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public bool IsNewUser { get; set; }
    }
}
