namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_history_changed")]
    public class ChannelHistoryChanged
    {
        public string latest;
        public string ts;
        public string event_ts;
    }
}