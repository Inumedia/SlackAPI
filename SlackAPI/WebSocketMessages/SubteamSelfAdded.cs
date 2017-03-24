namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("subteam_self_added")]
    public class SubteamSelfAdded
    {
        public string subteam_id;
    }
}