namespace SlackAPI
{
    // https://api.slack.com/methods/pins.list
    [RequestPath("pins.list")]
    public class PinListResponse : Response
    {
        public class PinItem
        {
            public string type; // message, file, file_comment
            public Message message; // message
            public string channel; // message
            public File file; // file, file_comment
            public string file_comment; // file_comment
            public long created;
        }

        public PinItem[] items;
    }
}
