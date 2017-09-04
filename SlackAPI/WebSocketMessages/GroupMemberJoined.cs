using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_join")]
    [SlackSocketRouting("message", "group_join")]
    public class GroupMemberJoined : SlackSocketMessage
    {
        public string user;
        public string channel;
        public DateTime ts;
        public string team;

        public GroupMemberJoined()
        {
            type = "group_join";
        }
    }
}
