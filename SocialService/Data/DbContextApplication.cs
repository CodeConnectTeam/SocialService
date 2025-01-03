using Microsoft.EntityFrameworkCore;

namespace SocialService.Data
{
    public class DbContextApplication : DbContext
    {
        public DbContextApplication(DbContextOptions<DbContextApplication> options) : base(options)
        {

        }

        public DbSet<InstagramPosts> InstagramPosts { get; set; }

        public DbSet<TwitterPosts> TwitterPosts { get; set; }
    }


}
