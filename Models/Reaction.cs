using System.Collections.Generic;

namespace SlackAPI.Models
{
    public class Reaction
    {
        public string name;
        public int count;
        public List<string> users;
    }
}
