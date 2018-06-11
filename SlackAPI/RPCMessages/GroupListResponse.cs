namespace SlackAPI
{
    [RequestPath("groups.list")]
    public class GroupListResponse : Response
    {
        public Channel[] groups;
    }
}
