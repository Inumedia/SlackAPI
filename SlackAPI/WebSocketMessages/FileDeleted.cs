namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_deleted")]
    public class FileDeleted
    {
        public string file_id;
        public string event_ts;
    }
}