namespace FacebookTimerPosts.DTOs
{
    public class PublishResultDto
    {
        public bool Success { get; set; }
        public string FacebookPostId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
