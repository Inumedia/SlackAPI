using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("groups.setPurpose")]
    public class GroupSetPurposeResponse : Response
    {
        public string purpose;
    }
}
