namespace SlackAPI.Models
{
    [RequestPath("auth.test", true)]
    public class AuthTestResponse : Response
    {
        public string url;
        public string team;
        public string user;
        public string team_id;
        public string user_id;
    }
}
