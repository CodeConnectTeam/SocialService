using Tweetinvi.Models.V2;

namespace SocialService.Models
{
    public class Tweet
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public TweetMetrics PublicMetrics { get; set; }
    }

    public class TweetMetrics
    {
        public int RetweetCount { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public int ImpressionCount { get; set; }
    }
}
