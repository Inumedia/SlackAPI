using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("presence_change")]
    public class PresenceChange : SlackSocketMessage
    {
        public string user;
        public Presence presence;
    }
}
