using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("groups.close")]
    public class GroupCloseResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
