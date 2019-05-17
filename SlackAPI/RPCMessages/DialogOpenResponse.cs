using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI.RPCMessages
{
   [RequestPath("dialog.open")]
   public class DialogOpenResponse : Response
   {
      public  ResponseMetadata response_metadata { get; set; }

      public class ResponseMetadata
      {
         public string[] messages { get; set; }
      }
   }
}
