using System.Text.Json.Serialization;

namespace SocialService.Models.InstagramModels
{
    public class GetMetricsResponse
    {
        [JsonPropertyName("data")]
        public List<InstagramMedia> Data { get; set; }
    }

    public class InstagramMedia
    {
        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        [JsonPropertyName("comments_count")]
        public int CommentsCount { get; set; }

        [JsonPropertyName("media_url")]
        public string MediaUrl { get; set; }

        [JsonPropertyName("permalink")]
        public string Permalink { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
