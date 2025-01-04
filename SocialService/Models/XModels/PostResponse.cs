using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SocialService.Models.XModels
{
    public class PostResponse
    {
        
        
            [JsonProperty("data")]
            public PostResponseTweet Data { get; set; }
        
    }

    public class PostResponseTweet
    {


        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("edit_history_tweet_ids")]
        public List<string>? EditHistoryTweetIds { get; set; }

    }
}
