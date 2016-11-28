namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_archive")]
    public class GroupArchive : SlackSocketMessage
    {
        public string channel;
    }
}

