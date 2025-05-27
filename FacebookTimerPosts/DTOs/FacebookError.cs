namespace FacebookTimerPosts.DTOs
{
    public class FacebookError
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public int Code { get; set; }
        public int ErrorSubcode { get; set; }
        public string FbtraceId { get; set; }
    }
}
