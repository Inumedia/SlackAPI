namespace SlackAPI.Models
{
    [RequestPath("groups.close")]
    public class GroupCloseResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
