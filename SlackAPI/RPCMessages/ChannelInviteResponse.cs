using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("channels.invite")]
    public class ChannelInviteResponse : Response
    {
        public Channel channel;
    }
}
