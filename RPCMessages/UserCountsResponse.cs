namespace SlackAPI.Models
{
    [RequestPath("users.counts")]
    public class UserCountsResponse : Response
    {
        public ChannelCounts[] channels;
        public ChannelCounts[] groups;
        public DirectMessageNewCount[] ims;
    }

    public class ChannelCounts
    {
        public string id;
        public int mention_count;
        public string name;
        public int unread_count;
    }

    public class DirectMessageNewCount
    {
        public int dm_count;
        public string id;
        public string name;
    }
}