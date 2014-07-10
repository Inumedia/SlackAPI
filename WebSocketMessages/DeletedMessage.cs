using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message", "message_deleted")]
    public class DeletedMessage : SlackSocketMessage
    {
        public string channel;
        public DateTime ts;
        public DateTime deleted_ts;
        public bool hidden;
    }
}
