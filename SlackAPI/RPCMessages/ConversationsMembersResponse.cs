namespace SlackAPI.RPCMessages
{
    [RequestPath("conversations.members")]
    public class ConversationsMembersResponse : Response
    {
        public string[] channels;
    }
}