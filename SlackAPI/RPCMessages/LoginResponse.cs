using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
	[RequestPath("rtm.start")]
	public class LoginResponse : Response
	{
		public Bot[] bots;
		public Channel[] channels;
		public Channel[] groups;
		public DirectMessageConversation[] ims;
		public Self self;
		public int svn_rev;
		public int min_svn_rev;
		public Team team;
		public string url;
		public User[] users;
	}

    public class Self
    {
        public DateTime created;
        public string id;
        public string manual_presence;
        public string name;
        public Preferences prefs;
    }
}
