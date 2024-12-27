using System.Text.Json.Serialization;

namespace SocialService.Models.InstagramModels
{
    public class ProfileResponse
    {
        [JsonPropertyName("data")]
        public InstagramProfile Data { get; set; }
    }

    public class InstagramProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Name { get; set; }
    }

}
