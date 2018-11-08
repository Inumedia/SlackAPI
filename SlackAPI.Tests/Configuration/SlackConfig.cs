namespace SlackAPI.Tests.Configuration
{
    public class SlackConfig
    {
        public string UserAuthToken { get; set; }
        public string BotAuthToken { get; set; }
        public string TestChannel { get; set; }
        public string DirectMessageUser { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string AuthUsername { get; set; }
        public string AuthPassword { get; set; }
        public string AuthWorkspace { get; set; }
    }
}