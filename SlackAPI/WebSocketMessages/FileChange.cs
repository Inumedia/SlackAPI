namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_change")]
    public class FileChange
    {
        public File file;
    }
}