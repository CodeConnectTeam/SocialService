using System.Text.Json.Serialization;

namespace SocialService.Models.InstagramModels
{
    public class CreatePostResponse
    {
        [JsonPropertyName("data")]
        public DraftPost Data { get; set; }
    }

    public class DraftPost
    {
        public string id { get; set; }
    }
}
