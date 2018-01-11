namespace SlackAPI
{
    // https://api.slack.com/methods/channels.setPurpose
    [RequestPath("channels.setPurpose")]
    public class ChannelSetPurposeResponse : Response
    {
        public string purpose;
    }
}
