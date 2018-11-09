namespace SlackAPI.RPCMessages
{
    [RequestPath("chat.postEphemeral")]
    public class PostEphemeralResponse : Response
    {
        public string message_ts;
    }
}
