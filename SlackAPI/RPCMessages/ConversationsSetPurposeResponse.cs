using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("conversations.setPurpose")]
    public class ConversationsSetPurposeResponse : Response
    {
        public string purpose;
    }
}
