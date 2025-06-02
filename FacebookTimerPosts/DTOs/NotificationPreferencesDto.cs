namespace FacebookTimerPosts.DTOs
{
    public class NotificationPreferencesDto
    {
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool PostSuccessNotifications { get; set; }
        public bool PostFailureNotifications { get; set; }
        public bool SubscriptionExpiryNotifications { get; set; }
        public bool SecurityAlerts { get; set; }
        public bool LoginAlerts { get; set; }
        public bool MarketingEmails { get; set; }
        public bool WeeklyReports { get; set; }
    }
}
