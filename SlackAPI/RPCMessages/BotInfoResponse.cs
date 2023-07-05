namespace SlackAPI.RPCMessages
{
    [RequestPath("bots.info")]
    public class BotInfoResponse : Response
    {
        public Bot bot;
    }
}
