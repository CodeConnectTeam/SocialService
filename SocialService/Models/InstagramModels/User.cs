namespace SocialService.Models.InstagramModels
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public AccessToken AccessToken { get; set; }
    }
    public class AccessToken
    {
        public List<string> Scope { get; set; }
        public bool State { get; set; } = true;
        public DateTime ExpirationDate { get; set; }
        public string AccessTokenLong { get; set; }

    }
}
