using System;

namespace SlackAPI.Models
{
    [RequestPath("rtm.start")]
	public class LoginResponse : Response
	{
		public Bot[] bots;
		public Channel[] channels;
		public Channel[] groups;
		public DirectMessage[] ims;
		public Self self;
		public int svn_rev;
		public int min_svn_rev;
		public Team team;

        /// <summary>
        /// The Websocket URLs provided by rtm.start are single-use and are only valid for 30 seconds, so make sure to connect quickly.
        /// </summary>
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
