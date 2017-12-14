using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class User
    {
        public string id;
        public bool IsSlackBot
        {
            get
            {
                return id.Equals("USLACKBOT", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        public string name;
        public bool deleted;
        public string color;
        public UserProfile profile;
        public bool is_admin;
        public bool is_owner;
        public bool is_primary_owner;
        public bool is_restricted;
        public bool is_ultra_restricted;
        public bool has_2fa;
        public string two_factor_type;
        public bool has_files;
        public string presence;
        public bool is_bot;
        public string tz;
        public string tz_label;
        public int tz_offset;
        public string team_id;
        public string real_name;
    }
}
