using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class SlackClientHelpers : SlackClientBase
    {
        public SlackClientHelpers()
        {
        }

        public SlackClientHelpers(IWebProxy proxySettings)
            : base(proxySettings)
        {
        }

        [Obsolete("Please use the OAuth method for authenticating users")]
        public void StartAuth(Action<AuthStartResponse> callback, string email)
        {
            APIRequest(callback, new Tuple<string, string>[] { new Tuple<string, string>("email", email) }, new Tuple<string, string>[0]);
        }

        [Obsolete("Please use the OAuth method for authenticating users")]
        public Task<AuthStartResponse> StartAuthAsync(string email)
        {
            return APIRequestAsync<AuthStartResponse>(new Tuple<string, string>[] { new Tuple<string, string>("email", email) }, new Tuple<string, string>[0]);
        }

        public void FindTeam(Action<FindTeamResponse> callback, string team)
        {
            //This seems to accept both 'team.slack.com' and just plain 'team'.
            //Going to go with the latter.
            Tuple<string, string> domainName = new Tuple<string, string>("domain", team);
            APIRequest(callback, new Tuple<string, string>[] { domainName }, new Tuple<string, string>[0]);
        }

        public Task<FindTeamResponse> FindTeamAsync(string team)
        {
            //This seems to accept both 'team.slack.com' and just plain 'team'.
            //Going to go with the latter.
            Tuple<string, string> domainName = new Tuple<string, string>("domain", team);
            return APIRequestAsync<FindTeamResponse>(new Tuple<string, string>[] { domainName }, new Tuple<string, string>[0]);
        }

        public void AuthSignin(Action<AuthSigninResponse> callback, string userId, string teamId, string password)
        {
            APIRequest(callback, new Tuple<string, string>[] {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("team", teamId),
                new Tuple<string,string>("password", password)
            }, new Tuple<string, string>[0]);
        }

        public Task<AuthSigninResponse> AuthSigninAsync(string userId, string teamId, string password)
        {
            return APIRequestAsync<AuthSigninResponse>(new Tuple<string, string>[] {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("team", teamId),
                new Tuple<string,string>("password", password)
            }, new Tuple<string, string>[0]);
        }

        public Uri GetAuthorizeUri(string clientId, SlackScope scopes, string redirectUri = null, string state = null, string team = null)
        {
            string theScopes = BuildScope(scopes);

            return GetSlackUri("https://slack.com/oauth/authorize", new Tuple<string, string>[] { new Tuple<string, string>("client_id", clientId),
                new Tuple<string, string>("redirect_uri", redirectUri),
                new Tuple<string, string>("state", state),
                new Tuple<string, string>("scope", theScopes),
                new Tuple<string, string>("team", team)});
        }

        public void GetAccessToken(Action<AccessTokenResponse> callback, string clientId, string clientSecret, string redirectUri, string code)
        {
            APIRequest<AccessTokenResponse>(callback, new Tuple<string, string>[] { new Tuple<string, string>("client_id", clientId),
                new Tuple<string, string>("client_secret", clientSecret), new Tuple<string, string>("code", code),
                new Tuple<string, string>("redirect_uri", redirectUri) }, new Tuple<string, string>[] { });
        }

        public Task<AccessTokenResponse> GetAccessTokenAsync(string clientId, string clientSecret, string redirectUri, string code)
        {
            return APIRequestAsync<AccessTokenResponse>(new Tuple<string, string>[] { new Tuple<string, string>("client_id", clientId),
                new Tuple<string, string>("client_secret", clientSecret), new Tuple<string, string>("code", code),
                new Tuple<string, string>("redirect_uri", redirectUri) }, new Tuple<string, string>[] { });
        }

        private string BuildScope(SlackScope scope)
        {
            var builder = new StringBuilder();
            if ((int)(scope & SlackScope.Identify) != 0)
                builder.Append("identify");
            if ((int)(scope & SlackScope.Read) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("read");
            }
            if ((int)(scope & SlackScope.Post) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("post");
            }
            if ((int)(scope & SlackScope.Client) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("client");
            }
            if ((int)(scope & SlackScope.Admin) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("admin");
            }

            return builder.ToString();
        }
    }
}
