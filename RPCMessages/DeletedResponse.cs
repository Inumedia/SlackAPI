using System;

namespace SlackAPI.Models
{
    [RequestPath("chat.delete")]
    public class DeletedResponse : Response
    {
        public string channel;
        public DateTime ts;
    }
}
