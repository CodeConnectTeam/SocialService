using System.Text.Json.Serialization;

namespace SocialService.Models
{
    public class UserResponse
    {
        [JsonPropertyName("data")]
        public User data { get; set; }   
    }

    public class User
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? username { get; set; }
    }

}
