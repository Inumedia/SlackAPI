using System;

namespace SlackAPI
{
    [RequestPath("conversations.create")]
    public class ConversationsCreateResponse : Response
    {
        public Channel channel;
    }
}
