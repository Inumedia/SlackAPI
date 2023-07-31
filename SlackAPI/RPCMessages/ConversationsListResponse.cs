namespace SlackAPI.RPCMessages
{
	[RequestPath("conversations.list")]
	public class ConversationsListResponse : Response
	{
		public Channel[] channels;
	}
}
