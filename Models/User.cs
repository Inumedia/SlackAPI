using System;

namespace SlackAPI.Models
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
        public bool is_bot;
        public string tz;
        public string tz_label;
        public int tz_offset;
    }
}
