namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_left")]
    public class GroupLeft : SlackSocketMessage
    {
        public string channel;
    }
}

