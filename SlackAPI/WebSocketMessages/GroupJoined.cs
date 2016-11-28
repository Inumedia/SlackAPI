namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_joined")]
    public class GroupJoined : SlackSocketMessage
    {
        public Channel channel;
    }
}

