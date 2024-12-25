using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Tweetinvi.Models.V2;

namespace SocialService.Models


{


    public class TweetResponse
    {
        [JsonProperty("data")]
        public TweetMetrics Data { get; set; }
    }
    public class Tweet
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("public_metrics")]
        public TweetMetrics? PublicMetrics { get; set; }
    }

    public class TweetMetrics
    {
        [JsonPropertyName("tweet_count")]
        public int TweetCount { get; set; }

        [JsonPropertyName("reply_count")]
        public int ReplyCount { get; set; }

        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        [JsonPropertyName("quote_count")]
        public int QuoteCount { get; set; }

        [JsonPropertyName("bookmark_count")]
        public int BookmarkCount { get; set; }

        [JsonPropertyName("impression_count")]
        public int ImpressionCount { get; set; }
    }
}
