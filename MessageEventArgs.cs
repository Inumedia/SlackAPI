using System;
using SlackAPI.WebSocketMessages;

namespace SlackAPI
{
    public class MessageEventArgs : EventArgs
    {
        public SlackMessage Message { get; private set; }

        public MessageEventArgs(SlackMessage message)
        {
            Message = message;
        }
    }
}