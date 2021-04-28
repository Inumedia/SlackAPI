using System;

namespace SlackAPI
{
    [Obsolete]
    [RequestPath("im.list")]
    public class DirectMessageConversationListResponse : Response
    {
        public DirectMessageConversation[] ims;
    }
}
