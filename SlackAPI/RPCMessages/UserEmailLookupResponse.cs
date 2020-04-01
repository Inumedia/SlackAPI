namespace SlackAPI.RPCMessages
{
    [RequestPath("users.lookupByEmail")]
    public class UserEmailLookupResponse : Response
    {
        public User user;
    }
}