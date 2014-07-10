using System;

namespace SlackAPI
{
    [RequestPath("auth.start")]
    public class AuthStartResponse : Response
    {
        public string email;
        public string domain;
        public UserTeamCombo[] users;
        //string[] teams; //Not sure?
        /// <summary>
        /// Path to create a new team?
        /// </summary>
        public string create;
    }
}
