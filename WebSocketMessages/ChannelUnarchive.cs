namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_unarchive")]
    public class ChannelUnarchive
    {
        public string channel;
        public string user;
    }
}
