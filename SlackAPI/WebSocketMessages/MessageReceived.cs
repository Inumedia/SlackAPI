using System;

namespace SlackAPI.WebSocketMessages
{
    public class MessageReceived : SlackSocketMessage
    {
        public string text;
        public DateTime ts;
    }
}
