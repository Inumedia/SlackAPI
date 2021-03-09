using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("conversations.open")]
    public class ConversationsOpenResponse : Response
    {
        public string no_op;
        public string already_open;
    }
}
