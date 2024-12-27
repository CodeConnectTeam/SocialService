using System.Text.Json.Serialization;

namespace SocialService.Models.InstagramModels
{
    public class PublishPostResponse
    {
        [JsonPropertyName("data")]
        public PublishedPost Data { get; set; }
    }

    public class PublishedPost
    {
        public string id { get; set; }
    }

}
