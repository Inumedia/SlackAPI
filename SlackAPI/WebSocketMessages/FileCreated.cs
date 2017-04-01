namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_created")]
    public class FileCreated
    {
        public File file;
    }
}