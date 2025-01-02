namespace SocialService.Data
{
    public class Post
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduleDateTime { get; set; }
        public DateTime? PublishedDateTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public SocialMediaAccount SocialMediaAccount { get; set; }
        public PostAnalytics PostAnalytics { get; set; }
    }

}
