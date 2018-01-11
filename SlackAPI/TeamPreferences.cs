namespace SlackAPI
{
    public class TeamPreferences
    {
        public AuthMode auth_mode;
        public string[] default_channels;
        public bool display_real_names;
        public bool gateway_allow_irc_plain;
        public bool gateway_allow_irc_ssl;
        public bool gateway_allow_xmpp_ssl;
        public bool hide_referers;
        public int msg_edit_window_mins;
        public bool srvices_only_admins;
        public bool stats_only_admins;

        public enum AuthMode
        {
            normal,
            saml,
            google
        }
    }
}
