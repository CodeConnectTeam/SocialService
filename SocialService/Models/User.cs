namespace SocialService.Models
{
    public class UserResponse
    {
        public User User { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}
