namespace SlackAPI
{
    [RequestPath("channels.invite")]
    public class ChannelInviteResponse : Response
    {
        public Channel channel;
    }
}
