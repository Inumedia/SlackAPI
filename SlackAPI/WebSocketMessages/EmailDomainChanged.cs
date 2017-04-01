namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("email_domain_changed")]
    public class EmailDomainChanged
    {
        public string email_domain;
        public string event_ts;
    }
}