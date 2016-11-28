namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message", "file_share")]
    public class FileShareMessage : NewMessage
    {
        public bool upload;

        public File file;
    }
}
