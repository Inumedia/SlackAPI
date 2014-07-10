using System;

namespace SlackAPI.WebSocketMessages
{
    class MessageReceived : SlackSocketMessage
    {
        public string text;
        public DateTime ts;
    }
}
