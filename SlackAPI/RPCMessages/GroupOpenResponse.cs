using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("groups.open")]
    public class GroupOpenResponse : Response
    {
        public string no_op;
        public string already_closed;
    }
}
