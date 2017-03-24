namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("subteam_self_removed")]
    public class SubteamSelfRemoved
    {
        public string subteam_id;
    }
}