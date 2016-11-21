namespace SlackAPI.Models
{
    [RequestPath("groups.setTopic")]
    public class GroupSetTopicResponse : Response
    {
        public string topic;
    }
}
