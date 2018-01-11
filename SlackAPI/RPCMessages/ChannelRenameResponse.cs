namespace SlackAPI
{
    // https://api.slack.com/methods/channels.rename
    [RequestPath("channels.rename")]
    public class ChannelRenameResponse : Response
    {
        public string name;
        public string created;
    }
}
