namespace SlackAPI.Models
{
    [RequestPath("files.list")]
    public class FileListResponse : Response
    {
        public File[] files;
        public PaginationInformation paging;
    }
}
