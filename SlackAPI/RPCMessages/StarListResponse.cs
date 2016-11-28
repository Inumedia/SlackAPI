using System;

namespace SlackAPI
{
    [RequestPath("stars.list")]
    public class StarListResponse : Response
    {
        public Star[] items;
        public PaginationInformation paging;
    }

    public class Star
    {
        public string type;
        public string channel;
        public Message message;
        public string file;
        public FileComment comment;
    }
}
