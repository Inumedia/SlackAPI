namespace SlackAPI
{
    [RequestPath("chat.scheduleMessage")]
    public class ScheduleMessageResponse : Response
    {
        public string ts;
        public string channel;
        public string scheduled_message_id;
        public int post_at;
        public Message message;

        public class Message
        {
            public string text;
            public string user;
            public string username;
            public string type;
            public string subtype;
        }
    }
}
