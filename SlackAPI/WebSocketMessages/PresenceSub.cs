namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("presence_sub")]
    public class PresenceSub : SlackSocketMessage
    {
        public string[] ids;
    }
}
