namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("im_history_changed")]
    public class ImHistoryChanged
    {
        public string latest;
        public string ts;
        public string event_ts;
    }
}