using System;
using System.Collections.Generic;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message")]
    [SlackSocketRouting("message", "bot_message")]
    public class NewMessage : SlackSocketMessage
    {
        public string user;
        public string channel;
        public string text;
        public string team;
        public DateTime ts;
        public DateTime thread_ts;
        public string username;
        public string bot_id;
        public UserProfile icons;
        public List<Attachment> attachments;

        public NewMessage()
        {
            type = "message";
        }
    }
}
