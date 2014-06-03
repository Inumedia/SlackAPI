using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("groups.list")]
    public class GroupListResponse : Response
    {
        public Channel[] groups;
    }
}
