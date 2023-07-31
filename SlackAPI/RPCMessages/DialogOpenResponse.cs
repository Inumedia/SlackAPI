namespace SlackAPI.RPCMessages
{
   [RequestPath("dialog.open")]
   public class DialogOpenResponse : Response
   {
      public new ResponseMetadata response_metadata { get; set; }

      public class ResponseMetadata
      {
         public string[] messages { get; set; }
      }
   }
}
