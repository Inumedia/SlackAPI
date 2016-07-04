using System;

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

        public NewMessage()
        {
            type = "message";
        }
    }
}
