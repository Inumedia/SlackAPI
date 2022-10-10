using System;

namespace SlackAPI
{
    [RequestPath("channels.list")]
    [Obsolete("Replaced by ConversationsListResponse", true)]
    public class ChannelListResponse : Response
    {
        public Channel[] channels;
    }
}
