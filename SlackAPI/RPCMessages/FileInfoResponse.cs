namespace SlackAPI
{
    [RequestPath("files.info")]
    public class FileInfoResponse : Response
    {
        public File file;
        public FileComment[] comments;
        public PaginationInformation paging;
    }
}
