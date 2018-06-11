using System;

namespace SlackAPI
{
    [RequestPath("chat.delete")]
    public class DeletedResponse : Response
    {
        public string channel;
        public DateTime ts;
    }
}
