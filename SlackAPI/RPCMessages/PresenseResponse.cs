using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("users.setPresence")]
    public class PresenceResponse : Response
    {
    }
    public enum Presence
    {
        active,
        away
    }
}
