using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("files.list")]
    public class FileListResponse : Response
    {
        public File[] files;
        public PaginationInformation paging;
    }
}
