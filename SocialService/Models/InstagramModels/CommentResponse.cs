using System.Text.Json.Serialization;

namespace SocialService.Models.InstagramModels
{
    public class InstagramCommentsResponse
    {
        [JsonPropertyName("data")]
        public List<InstagramComment> Data { get; set; }

        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }
    }

    public class InstagramComment
    {
        
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        
        [JsonPropertyName("replies")]
        public Replies Replies { get; set; }
    }

    public class Replies
    {
        [JsonPropertyName("data")]
        public List<InstagramComment> Data { get; set; }

        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        [JsonPropertyName("cursors")]
        public Cursors Cursors { get; set; }
    }

    public class Cursors
    {
        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("after")]
        public string After { get; set; }
    }

}
