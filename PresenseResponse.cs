using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("presense.set")]
    public class PresenceResponse : Response
    {
    }
    public enum Presence
    {
        Active,
        Away
    }
}
