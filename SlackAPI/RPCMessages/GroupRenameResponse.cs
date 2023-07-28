namespace SlackAPI
{
    [RequestPath("groups.rename")]
    public class GroupRenameResponse : Response
    {
        public Channel channel; 
    }
}
