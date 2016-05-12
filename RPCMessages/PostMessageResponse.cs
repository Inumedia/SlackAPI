namespace SlackAPI
{
    [RequestPath("chat.postMessage")]
    public class PostMessageResponse : Response
    {
        public string ts;
        public string channel;
        public Message message;

        public class Message
        {
            public string text;
            public string user;
            public string username;
            public string type;
            public string subtype;
            public string ts;
        }
    }
}
