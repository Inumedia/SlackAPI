using SlackAPI.WebSocketMessages;
using System;
using System.Diagnostics;
using System.Threading;

namespace SlackAPI
{
    public class SlackSocketClient : SlackClient
    {
        SlackSocket underlyingSocket;

        bool HelloReceived;
        public const int PingInterval = 3000;
        int pinging;
        Timer pingingThread;

        public long PingRoundTripMilliseconds { get; private set; }
        public bool IsReady { get { return HelloReceived; } }
        public bool IsConnected { get { return underlyingSocket != null && underlyingSocket.Connected; } }

        public event Action OnHello;

        public SlackSocketClient(string token)
            : base(token)
        {
            
        }

        protected override void Connected(LoginResponse loginDetails)
        {
            underlyingSocket = new SlackSocket(loginDetails, this);
        }

        public void BindCallback<K>(Action<K> callback)
        {
            underlyingSocket.BindCallback(callback);
        }

        public void UnbindCallback<K>(Action<K> callback)
        {
            underlyingSocket.UnbindCallback(callback);
        }

        public void SendPresence(Presence status)
        {
            underlyingSocket.Send(new PresenceChange() { presence = Presence.Active, user = base.MySelf.id });
        }

        public void SendTyping(string channelId)
        {
            underlyingSocket.Send(new Typing() { channel = channelId });
        }

        public void SendMessage(Action<MessageReceived> onSent, string channelId, string textData)
        {
            underlyingSocket.Send(new NewMessage() { channel = channelId, text = textData }, new Action<MessageReceived>((mr) =>
            {
                if (onSent != null)
                    onSent(mr);
            }));
        }

        public void HandleHello(Hello hello)
        {
            HelloReceived = true;

            StartPing();

            if (OnHello != null)
                OnHello();
        }

        public void HandlePresence(PresenceChange change)
        {
            UserLookup[change.user].presence = change.presence.ToString().ToLower();
        }

        void StartPing()
        {
            pingingThread = new Timer(Ping, null, PingInterval, PingInterval);
        }

        void Ping(object state)
        {
            if (Interlocked.CompareExchange(ref pinging, 1, 0) == 0)
            {
                //This isn't ideal.
                //TODO: Setup a callback on the messages so I get a hit when the message is just being sent. Messages are currently handled async.
                Stopwatch w = Stopwatch.StartNew();
                underlyingSocket.Send(new Ping()
                {
                    ping_interv_ms = PingInterval
                }, new Action<Pong>((p) =>
                {
                    w.Stop();
                    PingRoundTripMilliseconds = w.ElapsedMilliseconds;
                    pinging = 0;
                }));
            }
        }

        public void UserTyping(Typing t)
        {

        }

        public void Message(NewMessage m)
        {

        }

        public void PresenceChange(PresenceChange p)
        {

        }

        public void ChannelMarked(ChannelMarked m)
        {
            
        }
    }
}
