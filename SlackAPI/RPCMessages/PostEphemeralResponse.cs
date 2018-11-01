using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI.RPCMessages
{
    [RequestPath("chat.postEphemeral")]
    public class PostEphemeralResponse : Response
    {
        public string message_ts;
        public string channel;
    }
}
