using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialService.Data
{
    public class twitter_post
    {
        [Key]
        public int id { get; set; }

        public string? platform_id { get; set; }

        public string? tweet_text { get; set; }

        [StringLength(50)]
        public string? status { get; set; }

        public DateTime? created_at { get; set; } = DateTime.Now;

        public int? TweetCount { get; set; }

        public int? ReplyCount { get; set; }

        public int? LikeCount { get; set; }

        public int? QuoteCount { get; set; }

        public int? BookmarkCount { get; set; }

        public int? ImpressionCount { get; set; }
        public DateTime scheduled_time { get; set; }
    }
}
