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
        public string user;

        public bool is_archived;
        public bool is_member;
        public bool is_general;
        public bool is_channel;
        public bool is_group;
        public bool is_im;
        public bool is_mpim;
        public bool is_private;
        public bool is_shared;
        public bool is_org_shared;
        public bool is_ext_shared;
        public bool is_pending_ext_shared;

        public int num_members;
        public OwnedStampedMessage topic;
        public OwnedStampedMessage purpose;

        public string[] members;
    }
}
