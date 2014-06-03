using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("files.upload")]
    public class FileUploadResponse : Response
    {
        public File file;
    }
}
