namespace SocialService.Data
{
    public class Platform
    {
        public int PlatformID { get; set; }
        public string Name { get; set; }
        public string APIEndpoint { get; set; }

        public ICollection<SocialMediaAccount> SocialMediaAccounts { get; set; }
        public ICollection<PostAnalytics> PostAnalytics { get; set; }
    }

}
