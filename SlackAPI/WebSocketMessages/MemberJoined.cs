using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_join")]
    [SlackSocketRouting("message", "channel_join")]
    public class MemberJoined : SlackSocketMessage
    {
        public string user;
        public string channel;
        public DateTime ts;
        public string team;

        public MemberJoined()
        {
            type = "channel_join";
        }
    }
}
