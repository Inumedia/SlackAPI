namespace SlackAPI
{
    [RequestPath("conversations.open")]
    public class JoinDirectMessageChannelResponse : Response
    {
        public Channel channel;
    }
}