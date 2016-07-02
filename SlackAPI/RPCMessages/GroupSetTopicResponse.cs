using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("groups.setTopic")]
    public class GroupSetTopicResponse : Response
    {
        public string topic;
    }
}
