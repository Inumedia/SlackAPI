using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI.RPCMessages
{
	[RequestPath("conversations.list")]
	public class ConversationsListResponse : Response
	{
		public Channel[] channels;
	}
}
