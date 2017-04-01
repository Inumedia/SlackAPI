namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("group_history_changed")]
    public class GroupHistoryChanged
    {
        public string latest;
        public string ts;
        public string event_ts;
    }
}