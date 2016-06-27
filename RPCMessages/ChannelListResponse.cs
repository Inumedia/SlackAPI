namespace SlackAPI.Models
{
    [RequestPath("channels.list")]
    public class ChannelListResponse : Response
    {
        public Channel[] channels;
    }
}
