using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
	[RequestPath("rtm.connect")]
	public class LoginResponse : Response
    {
        public string url;
        public Team team;
        public Self self;
    }

    public class Self
    {
        public string id;
        public string name;
    }
}
