namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("im_open")]
    public class ImOpen
    {
        public string user;
        public string channel;
    }
}