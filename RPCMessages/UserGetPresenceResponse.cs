﻿namespace SlackAPI.Models
{
    [RequestPath("users.getPresence")]
    public class UserGetPresenceResponse : Response
    {
        public Presence presence;
    }
}
