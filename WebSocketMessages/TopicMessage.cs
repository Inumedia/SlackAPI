namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message", "channel_topic")]
    public class TopicMessage : SlackMessage
    {
        public TopicMessage()
            : base("topic")
        {
        }
    }
}