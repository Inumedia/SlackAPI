namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("user_change")]
    public class UserChange : SlackSocketMessage
    {
        public User user;
    }
}

