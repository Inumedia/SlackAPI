namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_comment_deleted")]
    public class FileCommentDeleted
    {
        public File file;
        public string comment;
    }
}