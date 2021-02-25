using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("conversations.close")]
    public class ConversationsCloseResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
