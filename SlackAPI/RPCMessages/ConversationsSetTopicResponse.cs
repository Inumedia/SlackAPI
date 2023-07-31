namespace SlackAPI
{
    [RequestPath("conversations.setTopic")]
    public class ConversationsSetTopicResponse : Response
    {
        public string topic;
    }
}
