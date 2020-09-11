using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
	public class Conversation
	{
		public string id;
		public DateTime created;
		public DateTime last_read;
		public bool is_open;
		public bool is_starred;
		public int unread_count;
		public Message latest;

		public static string ConversationTypesToQueryParam(ConversationTypes[] types)
		{
			//Translate the enum user-friendly names to API used names
			List<string> typesAsString = new List<string>();
			if (types.Contains(ConversationTypes.PublicChannel))
			{
				typesAsString.Add("public_channel");
			}
			if (types.Contains(ConversationTypes.PrivateChannel))
			{
				typesAsString.Add("private_channel");
			}
			if (types.Contains(ConversationTypes.GroupMessage))
			{
				typesAsString.Add("mpim");
			}
			if (types.Contains(ConversationTypes.DirectMessage))
			{
				typesAsString.Add("im");
			}

			return string.Join(",", types);
		}
	}

	public enum ConversationTypes
	{
		PublicChannel, //public_channel
		PrivateChannel, //private_channel
		GroupMessage, //mpim
		DirectMessage //im
	}
}
