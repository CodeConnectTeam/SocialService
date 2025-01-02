using Microsoft.Extensions.Hosting;

namespace SocialService.Data
{
    public class SocialMediaAccount
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public int PlatformID { get; set; }
        public string AccessToken { get; set; }
        public string AccountUsername { get; set; }
        public DateTime LinkedDate { get; set; }

        public User User { get; set; }
        public Platform Platform { get; set; }
        public ICollection<Post> Posts { get; set; }
    }

}
