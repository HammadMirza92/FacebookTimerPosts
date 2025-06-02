namespace FacebookTimerPosts.DTOs
{
    public class UserLoginInfoDto
    {
        public string LoginProvider { get; set; } = string.Empty;
        public string ProviderDisplayName { get; set; } = string.Empty;
        public DateTime? ConnectedDate { get; set; }
    }
}
