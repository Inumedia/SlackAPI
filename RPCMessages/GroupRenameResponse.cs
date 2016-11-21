namespace SlackAPI.Models
{
    [RequestPath("groups.rename")]
    public class GroupRenameResponse : Response
    {
        public Channel channel; 
    }
}
