namespace SlackAPI.RPCMessages
{
    
    [RequestPath("conversations.join")]
    public class ConversationsJoinResponse : Response
    {
        public Channel channel;
    }
}