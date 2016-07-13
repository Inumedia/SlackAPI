using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class Conversation
    {
        public string id;
        public DateTime created;
        public DateTime last_read;
        public bool is_open;
        public bool is_starred;
        public int unread_count;
        public Message latest;
    }
}
