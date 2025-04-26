namespace FacebookTimerPosts.DTOs
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int MaxPosts { get; set; }
        public int MaxTemplates { get; set; }
        public bool IsActive { get; set; }
    }
}
