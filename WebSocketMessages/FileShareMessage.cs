namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message", "file_share")]
    public class FileShareMessage : SlackMessage
    {
        public bool upload;

        public File file;
    }
}
