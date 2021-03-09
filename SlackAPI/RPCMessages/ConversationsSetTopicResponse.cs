using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("conversations.setTopic")]
    public class ConversationsSetTopicResponse : Response
    {
        public string topic;
    }
}
