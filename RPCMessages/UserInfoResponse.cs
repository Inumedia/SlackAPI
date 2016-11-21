namespace SlackAPI.Models
{
    [RequestPath("users.info")]
    public class UserInfoResponse : Response
    {
        public User user;
    }
}
