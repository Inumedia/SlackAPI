using System.Linq;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("presence_sub")]
    public class PresenceChangeSubscription : SlackSocketMessage
    {
        public PresenceChangeSubscription(string[] usersIds)
        {
            this.ids = usersIds;
        }

        public string[] ids { get; }
    }
}
