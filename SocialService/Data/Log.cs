namespace SocialService.Data
{
    public class Log
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }

        public User User { get; set; }
    }

}
