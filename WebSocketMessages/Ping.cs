using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("ping")]
    public class Ping : SlackSocketMessage
    {
        public int ping_interv_ms = 3000;
    }
}
