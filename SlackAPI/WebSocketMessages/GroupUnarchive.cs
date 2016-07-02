namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_unarchive")]
    public class GroupUnarchive : SlackSocketMessage
    {
        public string channel;
    }
}

