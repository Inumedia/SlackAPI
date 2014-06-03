using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
