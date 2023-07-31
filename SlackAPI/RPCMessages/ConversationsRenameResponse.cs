namespace SlackAPI
{
    [RequestPath("conversations.rename")]
    public class ConversationsRenameResponse : Response
    {
        public Channel channel; 
    }
}
