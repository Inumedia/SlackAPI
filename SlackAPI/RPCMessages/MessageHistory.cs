using System;
using Newtonsoft.Json.Serialization;

namespace SlackAPI
{
    public abstract class MessageHistory : Response
    {
        /// <summary>
        /// I believe this is where the read cursor is?  IE: How far the user has read.
        /// </summary>
        public DateTime latest;
        public Message[] messages;
        public bool has_more;
        public int unread_count_display;

        public bool channel_not_found;
        public bool invalid_ts_latest;
        public bool invalid_ts_oldest;
    }
}
