using Microsoft.Extensions.Hosting;
using System.Data;

namespace SocialService.Data
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public DateTime LastLoginDate { get; set; }

        public Role Role { get; set; }
        public ICollection<SocialMediaAccount> SocialMediaAccounts { get; set; }
        public ICollection<Log> Logs { get; set; }
        public ICollection<Post> Posts { get; set; }
    }

}
