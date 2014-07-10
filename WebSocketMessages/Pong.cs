using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("pong")]
    public class Pong : SlackSocketMessage
    {
        public int ping_interv_ms;
    }
}
