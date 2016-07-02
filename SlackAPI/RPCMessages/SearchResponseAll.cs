using System;

namespace SlackAPI
{
    [RequestPath("search.all")]
    public class SearchResponseAll : Response
    {
        public string query;
        public SearchResponseMessagesContainer messages;
        public SearchResponseFilesContainer files;
    }
}
