namespace SlackAPI
{
    [RequestPath("channels.setTopic")]
    public class ChannelSetTopicResponse : Response
    {
        public string topic;
    }
}