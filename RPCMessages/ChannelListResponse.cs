using System;

namespace SlackAPI
{
    [RequestPath("channels.list")]
    public class ChannelListResponse : Response
    {
        public Channel[] channels;
    }
}
