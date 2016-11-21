namespace SlackAPI.Models
{
    [RequestPath("im.list")]
    public class DirectMessageConversationListResponse : Response
    {
        public DirectMessage[] ims;
    }
}
