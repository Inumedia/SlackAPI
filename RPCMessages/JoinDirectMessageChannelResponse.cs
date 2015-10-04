namespace SlackAPI
{
    [RequestPath("im.open")]
    public class JoinDirectMessageChannelResponse : Response
    {
        public Channel channel;
    }
}