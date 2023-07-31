namespace SlackAPI
{
    [RequestPath("groups.setTopic")]
    public class GroupSetTopicResponse : Response
    {
        public string topic;
    }
}
