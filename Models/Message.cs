using System;

namespace SlackAPI
{
    public class Message : SlackSocketMessage
    {
        public string channel;
        public DateTime ts;
        public string user;
        /// <summary>
        /// Isn't always set. Should look up if not set.
        /// </summary>
        public string username;
        public string text;
        public bool is_starred;
        public string permalink;
        public Reaction[] reactions;
        //Wibblr? Not really sure what this applies to.  :<
    }
}
