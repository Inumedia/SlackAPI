namespace SlackAPI.Models
{
    [RequestPath("channels.create")]
    public class ChannelCreateResponse : Response
    {
        public Channel channel;
    }
}
