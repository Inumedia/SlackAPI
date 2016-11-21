using SlackAPI.Models;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_created")]
    public class ChannelCreated
    {
        public Channel channel;
    }
}
