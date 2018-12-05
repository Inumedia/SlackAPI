namespace SlackAPI
{
    //See: https://api.slack.com/docs/message-menus
    public class AttachmentActionOption
    {
        public AttachmentActionOption(string text, string value)
        {
            this.text = text;
            this.value = value;
        }
        public string text { get; }
        public string value;
    }
}
