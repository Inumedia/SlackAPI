using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("auth.test", true)]
    public class AuthTestResponse : Response
    {
        public string url;
        public string team;
        public string user;
        public string team_id;
        public string user_id;
    }
}
