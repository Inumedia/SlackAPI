namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_close")]
    public class GroupClose : SlackSocketMessage
    {
        public string user;
        public string channel;
    }
}

