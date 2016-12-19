namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_archive")]
    public class ChannelArchive
    {
        public string channel;
        public string user;
    }
}
