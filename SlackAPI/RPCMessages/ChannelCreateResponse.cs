using System;

namespace SlackAPI
{
    [RequestPath("channels.create")]
    public class ChannelCreateResponse : Response
    {
        public Channel channel;
    }
}
