using System;

namespace SlackAPI
{
    public class Group
    {
        public string id;

        public string name;
        public bool is_group;
        public DateTime created;
        public string creator;
        public bool is_archived;
        public bool is_open;

        public string[] members;

        public OwnedStampedMessage topic;
        public OwnedStampedMessage purpose;

        public DateTime last_read;
        public Message latest;

        public int unread_count;
        public int unread_count_display;
    }
}
