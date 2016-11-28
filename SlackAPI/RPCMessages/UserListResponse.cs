using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("users.list")]
    public class UserListResponse : Response
    {
        public User[] members;
    }
}
