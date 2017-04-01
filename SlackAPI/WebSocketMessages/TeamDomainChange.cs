namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("team_domain_change")]
    public class TeamDomainChange
    {
        public string url;
        public string domain;
    }
}