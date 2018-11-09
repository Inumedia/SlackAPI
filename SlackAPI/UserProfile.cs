using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class UserProfile : ProfileIcons
    {
        public string first_name;
        public string last_name;
        public string real_name;
        public string email;
        public string skype;
        public string status_emoji;
        public string status_text;
        public string phone;

        public override string ToString()
        {
            return real_name;
        }
    }
}
