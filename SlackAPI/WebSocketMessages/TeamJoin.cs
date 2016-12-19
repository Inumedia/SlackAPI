namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("team_join")]
    public class TeamJoin : SlackSocketMessage
    {
        public User user;
    }
}
