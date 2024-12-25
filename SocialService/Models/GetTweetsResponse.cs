using Autofac.Features.Metadata;
using System.Text.Json.Serialization;

namespace SocialService.Models
{
    public class GetTweetsResponse
    {
        
        [JsonPropertyName("data")]
        public List<Tweet>? Tweets { get; set; }

        [JsonPropertyName("meta")]
        public Meta? Meta { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }

        [JsonPropertyName("newest_id")]
        public string? NewestId { get; set; }

        [JsonPropertyName("oldest_id")]
        public string? OldestId { get; set; }
    }
}
