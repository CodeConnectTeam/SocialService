namespace SocialService.Models.InstagramModels
{
    public class Comment
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public int LikeCount { get; set; }
        public List<Comment> Replies { get; set; }
    }
}
