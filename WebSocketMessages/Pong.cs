using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("pong")]
    class Pong : SlackSocketMessage
    {
        public int ping_interv_ms;
    }
}
