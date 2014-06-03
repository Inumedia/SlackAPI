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
        public bool has_files;
        public string presence;
    }
}
