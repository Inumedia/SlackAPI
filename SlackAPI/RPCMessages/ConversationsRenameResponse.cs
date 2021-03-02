using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("conversations.rename")]
    public class ConversationsRenameResponse : Response
    {
        public Channel channel; 
    }
}
