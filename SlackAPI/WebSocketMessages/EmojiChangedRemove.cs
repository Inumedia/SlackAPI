namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("emoji_changed", "remove")]
    public class EmojiChangedRemove
    {
        public string[] names;
        public string event_ts;
    }
}