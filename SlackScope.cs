using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [Flags]
    public enum SlackScope
    {
        Identify = 1,
        Read = 2,
        Post = 4,
        Client = 8,
        Admin = 16,
    }
}
