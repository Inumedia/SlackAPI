﻿namespace SlackAPI
{
    [RequestPath("search.files")]
    public class SearchResponseFiles : Response
    {
        public string query;
        SearchResponseMessagesContainer messages;
    }

    public class SearchResponseFilesContainer
    {
        /// <summary>
        /// Can be null if used in the context of search.all
        /// Please use paging.total instead.
        /// </summary>
        public int total;
        public PaginationInformation paging;
        public File[] matches;
    }
}
