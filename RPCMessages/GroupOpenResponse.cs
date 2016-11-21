namespace SlackAPI.Models
{
    [RequestPath("groups.open")]
    public class GroupOpenResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
