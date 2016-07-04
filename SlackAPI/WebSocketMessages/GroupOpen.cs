namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_open")]
    public class GroupOpen : SlackSocketMessage
    {
        public string user;
        public string channel;
    }
}

