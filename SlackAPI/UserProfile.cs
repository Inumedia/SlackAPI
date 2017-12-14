using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class UserProfile
    {
        public string first_name;
        public string last_name;
        public string real_name;
        public string email;
        public string skype;
        public string status_emoji;
        public string status_text;
        public string phone;
        public string image_24;
        public string image_32;
        public string image_48;
        public string image_72;
        public string image_192;

        public override string ToString()
        {
            return real_name;
        }
    }
}
