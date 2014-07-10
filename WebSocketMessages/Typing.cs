using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("typing")]
    class Typing : SlackSocketMessage
    {
        public string user;
        public string channel;
    }
}
