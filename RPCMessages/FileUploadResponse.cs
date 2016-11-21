namespace SlackAPI.Models
{
    [RequestPath("files.upload")]
    public class FileUploadResponse : Response
    {
        public File file;
    }
}
