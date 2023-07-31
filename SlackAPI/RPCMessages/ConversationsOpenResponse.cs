namespace SlackAPI
{
    [RequestPath("conversations.open")]
    public class ConversationsOpenResponse : Response
    {
        public string no_op;
        public string already_open;
        public Channel channel;
    }
}
