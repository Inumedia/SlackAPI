using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message_changed")]
    [SlackSocketRouting("message", "message_changed")]
    public class UpdatedMessage : SlackSocketMessage
    {
        public string user;
        public string channel;
        public string text;
        public string team;
        public DateTime ts;
        public DateTime thread_ts;
        public NewMessage message;

        public UpdatedMessage()
        {
            type = "message_changed";
        }
    }
}
