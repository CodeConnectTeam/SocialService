using Microsoft.EntityFrameworkCore;

namespace SocialService.Data
{
    public class DbContextApplication : DbContext
    {
        public DbContextApplication(DbContextOptions<DbContextApplication> options) : base(options)
        {

        }

        public DbSet<instagram_post> instagram_posts { get; set; }

        public DbSet<twitter_post> twitter_posts { get; set; }
    }


}
