namespace SlackAPI
{
    [RequestPath("conversations.setPurpose")]
    public class ConversationsSetPurposeResponse : Response
    {
        public string purpose;
    }
}
