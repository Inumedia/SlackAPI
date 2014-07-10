using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("typing")]
    public class Typing : SlackSocketMessage
    {
        public string user;
        public string channel;
    }
}
