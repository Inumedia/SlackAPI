using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message")]
    public class NewMessage : SlackSocketMessage
    {
        public string channel;
        public string text;
    }
}
