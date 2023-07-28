namespace SlackAPI
{
    [RequestPath("conversations.close")]
    public class ConversationsCloseResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
