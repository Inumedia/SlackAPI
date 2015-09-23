namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_rename")]
    public class GroupRename : SlackSocketMessage
    {
        public Channel channel;
    }
}

