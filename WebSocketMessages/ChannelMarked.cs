using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_marked")]
    public class ChannelMarked : SlackSocketMessage
    {
        public string channel;
        public DateTime ts;
    }
}
