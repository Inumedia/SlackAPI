using System;

namespace SlackAPI
{
    [Obsolete]
    [RequestPath("channels.list")]
    public class ChannelListResponse : Response
    {
        public Channel[] channels;
    }
}
