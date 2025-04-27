namespace FacebookTimerPosts.DTOs
{
    public class ExternalAuthDto
    {
        public string Provider { get; set; } // "Google" or "Facebook"
        public string IdToken { get; set; }  // Token received from the provider
    }
}
