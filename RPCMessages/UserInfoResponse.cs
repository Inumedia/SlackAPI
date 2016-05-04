namespace SlackAPI.RPCMessages
{
    [RequestPath("users.info")]
    public class UserInfoResponse : Response
    {
        public User user;
    }
}
