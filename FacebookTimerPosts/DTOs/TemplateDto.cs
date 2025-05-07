namespace FacebookTimerPosts.DTOs
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string DefaultFontFamily { get; set; }
        public string PrimaryColor { get; set; }
        public bool RequiresSubscription { get; set; }
        public int? MinimumSubscriptionPlanId { get; set; }
        public string MinimumSubscriptionPlanName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // HTML template content field
        public string HtmlTemplate { get; set; }

        // Available template variables for frontend substitution
        public List<string> TemplateVariables { get; set; } = new List<string>();
    }
}
