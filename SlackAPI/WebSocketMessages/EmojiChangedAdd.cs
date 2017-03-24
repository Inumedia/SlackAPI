namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("emoji_changed", "add")]
    public class EmojiChangedAdd
    {
        public string name;
        public string value;
        public string event_ts;
    }
}