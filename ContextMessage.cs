using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class ContextMessage : Message
    {
        //public string type;
        /// <summary>
        /// Only contains partial channel data.
        /// </summary>
        //public Channel channel;
        //public string user;
        //public string username;
        //public DateTime ts;
        //public string text;
        //public string permalink;
        public Message previous_2;
        public Message previous;
        public Message next;
        public Message next_2;
    }
}
