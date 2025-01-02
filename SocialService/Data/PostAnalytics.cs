namespace SocialService.Data
{
    public class PostAnalytics
    {
        public int AnalyticsID { get; set; }
        public int PostID { get; set; }
        public int PlatformID { get; set; }
        public int Likes { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public int Reach { get; set; }
        public DateTime CollectedDate { get; set; }

        public Post Post { get; set; }
        public Platform Platform { get; set; }
    }

}
