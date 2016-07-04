using System;

namespace SlackAPI
{
    [RequestPath("auth.signin")]
    public class AuthSigninResponse : Response
    {
        public string token;
        public string user;
        public string team;
    }
}
