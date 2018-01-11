using System;

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
