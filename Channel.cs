using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class Channel
    {
        public string id;

        public string name;
        public string creator;
        public DateTime created;
        public DateTime last_read;

        public bool is_archived;
        public bool is_member;
        public bool is_general;
        public bool is_starred;
        public bool is_channel;
        public bool is_group;
        public bool is_open;
        //Is this deprecated by is_open?
        public bool IsPrivateGroup { get { return id != null && id[0] == 'G'; } }

        public int num_members;
        public int unread_count;
        public OwnedStampedMessage topic;
        public OwnedStampedMessage purpose;

        public Message latest;

        public string[] members;
    }
}
