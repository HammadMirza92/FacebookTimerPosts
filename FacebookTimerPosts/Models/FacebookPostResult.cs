namespace FacebookTimerPosts.Models
{
    public class FacebookPostResult
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string PostId { get; set; }
        public string ErrorMessage { get; set; }

        public FacebookPostResult()
        {
        }

        public FacebookPostResult(bool success, string postId, string errorMessage)
        {
            Success = success;
            PostId = postId;
            ErrorMessage = errorMessage;
        }

        // Static helper methods for easy creation
        public static FacebookPostResult CreateSuccess(string postId)
        {
            return new FacebookPostResult(true, postId, null);
        }

        public static FacebookPostResult CreateFailure(string errorMessage)
        {
            return new FacebookPostResult(false, null, errorMessage);
        }
    }
}
