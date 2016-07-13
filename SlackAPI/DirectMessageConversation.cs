using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class DirectMessageConversation : Conversation
    {
        public string user;
        public bool is_user_deleted;
    }
}
