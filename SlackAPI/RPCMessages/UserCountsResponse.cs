using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
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