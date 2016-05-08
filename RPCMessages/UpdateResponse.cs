namespace SlackAPI
{
    [RequestPath("chat.update")]
    public class UpdateResponse : Response
    {
        public string channel;
        public string ts;
        public string text;
        public Message message;

        public class Message
        {
            public string type;
            public string user;
            public string text;
        }
    }
}