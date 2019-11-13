using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("user_typing")]
    public class UserTyping : SlackSocketMessage
    {
        public string user;
        public string channel;
    }
}
