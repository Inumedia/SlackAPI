namespace SlackAPI.Models
{
    [RequestPath("channels.setTopic")]
    public class ChannelSetTopicResponse : Response
    {
        public string topic;
    }
}