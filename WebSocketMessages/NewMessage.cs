using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message")]
    class NewMessage : SlackSocketMessage
    {
        public string channel;
        public string text;
    }
}
