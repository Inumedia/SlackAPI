namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("im_close")]
    public class ImClosed
    {
        public string user;
        public string channel;
    }
}