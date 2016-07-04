namespace SlackAPI.RPCMessages
{
    [RequestPath("users.getPresence")]
    public class UserGetPresenceResponse : Response
    {
        public Presence presence;
    }
}
