namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message", "me_message")]
    public class ActionMessage : SlackMessage
    {
        public ActionMessage()
            : base("action")
        {
        }
    }
}