namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_comment_edited")]
    public class FileCommentEdited
    {
        public File file;
        public string comment;
    }
}