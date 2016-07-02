using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    /// <summary>
    /// This is an undocumented response from an undocumented API. If anyone finds more info on this, please create a pull request.
    /// </summary>
    [RequestPath("auth.findTeam")]
    public class FindTeamResponse : Response
    {
        public string sso_required, sso_type, team_id, url;
        public string[] email_domains;
        public bool sso;
        public SSOProvider[] sso_provider;

        public static implicit operator Team(FindTeamResponse resp)
        {
            Team end = new Team();
            end.sso_required = resp.sso_required;
            end.sso_type = resp.sso_type;
            end.id = resp.team_id;
            end.url = resp.url;
            end.sso = resp.sso;
            end.email_domains = resp.email_domains;
            end.sso_provider = resp.sso_provider;
            return end;
        }
    }
}
