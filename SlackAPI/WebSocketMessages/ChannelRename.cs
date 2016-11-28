namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_rename")]
    public class ChannelRename
    {
        public Channel channel;
    }
}
