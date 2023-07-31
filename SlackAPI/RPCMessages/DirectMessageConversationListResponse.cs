namespace SlackAPI
{
    [RequestPath("im.list")]
    public class DirectMessageConversationListResponse : Response
    {
        public DirectMessageConversation[] ims;
    }
}
