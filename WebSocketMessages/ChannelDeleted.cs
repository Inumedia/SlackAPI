namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_deleted")]
    public class ChannelDeleted
    {
        public string channel;
    }
}
