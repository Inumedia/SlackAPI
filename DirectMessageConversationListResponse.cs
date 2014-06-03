using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("im.list")]
    public class DirectMessageConversationListResponse : Response
    {
        public DirectMessageConversation[] ims;
    }
}
