using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    /// <summary>
    /// This is used for moving the read cursor in the channel.
    /// </summary>
    [RequestPath("channels.marks")]
    public class MarkResponse : Response
    {
    }
}
