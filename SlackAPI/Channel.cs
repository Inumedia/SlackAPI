using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class Channel : Conversation
    {
        public string name;
        public string creator;

        public bool is_archived;
        public bool is_member;
        public bool is_general;
        public bool is_channel;
        public bool is_group;
        //Is this deprecated by is_open?
        public bool IsPrivateGroup { get { return id != null && id[0] == 'G'; } }

        public int num_members;
        public OwnedStampedMessage topic;
        public OwnedStampedMessage purpose;

        public string[] members;
    }
}
