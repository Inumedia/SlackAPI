using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("chat.delete")]
    public class DeletedResponse : Response
    {
        public string channel;
        public DateTime ts;
    }
}
