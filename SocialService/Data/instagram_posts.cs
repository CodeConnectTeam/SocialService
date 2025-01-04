using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SocialService.Data
{
    public class instagram_posts
    {
        [Key]
        public int id { get; set; }

        public string platform_id { get; set; }

        public string? image_url { get; set; }

        public string? caption { get; set; }

        [StringLength(50)]
        public string? media_type { get; set; }

        [StringLength(50)]
        public string? status { get; set; }

        public int? like_count { get; set; }

        public int? comment_count { get; set; }

        public DateTime? created_at { get; set; } = DateTime.Now;
    }
}
