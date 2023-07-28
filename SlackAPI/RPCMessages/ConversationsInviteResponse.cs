namespace SlackAPI
{
    [RequestPath("conversations.invite")]
    public class ConversationsInviteResponse : Response
    {
        public Channel channel;
    }
}
