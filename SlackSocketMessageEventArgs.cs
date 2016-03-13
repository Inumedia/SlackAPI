using System;

namespace SlackAPI
{
    public class SlackSocketMessageEventArgs : EventArgs
    {
        public SlackSocketMessage Message { get; private set; }

        public SlackSocketMessageEventArgs(SlackSocketMessage message)
        {
            Message = message;
        }
    }
}