using System.Net.WebSockets;
using System;
using System.Threading;
using SlackAPI.WebSocketMessages;
using SlackAPI.Models;

namespace SlackAPI
{
    public class SlackSocketClient : SlackClient
    {
        SlackSocket underlyingSocket;

        public event Action<NewMessage, SlackSocketClient> OnMessageReceived;
        public event Action<UserChange, SlackSocketClient> OnUserChange;
        public event Action<PresenceChange, SlackSocketClient> OnPresenceChange;
        public event Action<ChannelRename, SlackSocketClient> OnChannelRename;
        public event Action<Typing, SlackSocketClient> OnTyping;
        public event Action<ChannelMarked, SlackSocketClient> OnChannelMarked;
        public event Action<Hello, SlackSocketClient> OnHello;
        public event Action<ReactionAdded, SlackSocketClient> OnReactionAdded;
        public event Action<Pong, SlackSocketClient> OnPongReceived;
        public event Action<SlackSocketClient> OnConnectionClosed;

        /// <summary>
        /// When the Receiving Thread/task from the socket have a error and finish
        /// </summary>
        public event Action<Exception, SlackSocketClient> ErrorLoopSocket;

        bool HelloReceived;
        public const int PingInterval = 3000;
        int pinging;
        Timer pingingThread;

        public long PingRoundTripMilliseconds { get; private set; }
        public bool IsReady { get { return HelloReceived; } }
        public bool IsConnected { get { return underlyingSocket != null && underlyingSocket.Connected; } }

        internal LoginResponse loginDetails;

        public SlackSocketClient(string token)
            : base(token)
        {

        }

        public override void Connect(Action<LoginResponse, SlackClient> onConnected, Action onSocketConnected = null)
        {
            EmitLogin((loginDetails) =>
            {
                if (loginDetails.Ok)
                {
                    Connected(loginDetails);
                    ConnectSocket(onSocketConnected);
                }
                if (onConnected != null)
                    onConnected(loginDetails, this);
            });
        }

        protected override void Connected(LoginResponse loginDetails)
        {
            this.loginDetails = loginDetails;
            base.Connected(loginDetails);
        }

        public void ConnectSocket(Action onSocketConnected)
        {
            underlyingSocket = new SlackSocket(loginDetails, this, onSocketConnected);
            underlyingSocket.ConnectionClosed += UnderlyingSocketConnectionClosed;
            underlyingSocket.ErrorLoopSocket += UnderlyingSocketErrorLoopSocket;
        }

        private void UnderlyingSocketErrorLoopSocket(Exception ex)
        {
            if (ErrorLoopSocket != null) ErrorLoopSocket(ex, this);
        }

        /// <summary>
        /// Invoke when the connection is closed
        /// </summary>
        private void UnderlyingSocketConnectionClosed()
        {
            if (OnConnectionClosed != null) OnConnectionClosed(this);
        }

        public void ErrorReceiving(Action<WebSocketException> callback)
        {
            if (callback != null) underlyingSocket.ErrorReceiving += callback;
        }

        public void ErrorReceivingDesiralization(Action<Exception> callback)
        {
            if (callback != null) underlyingSocket.ErrorReceivingDesiralization += callback;
        }

        public void ErrorHandlingMessage(Action<Exception> callback)
        {
            if (callback != null) underlyingSocket.ErrorHandlingMessage += callback;
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
            underlyingSocket.Send(new PresenceChange() { presence = Presence.active, user = base.MySelf.id });
        }

        public void SendTyping(string channelId)
        {
            underlyingSocket.Send(new Typing() { channel = channelId });
        }

        public void SendMessage(Action<MessageReceived> onSent, string channelId, string textData, string userName = null)
        {
            if (userName == null)
            {
                userName = MySelf.id;
            }

            if (onSent != null)
            {
                underlyingSocket.Send(new Message() { channel = channelId, text = textData, user = userName, type = "message" }, onSent);
            }
            else {
                underlyingSocket.Send(new Message() { channel = channelId, text = textData, user = userName, type = "message" });
            }
        }

        public void SendPing()
        {
            underlyingSocket.Send(new Ping());
        }

        public void HandlePongReceived(Pong pong)
        {
            if (OnPongReceived != null)
                OnPongReceived(pong, this);
        }

        public void HandleReactionAdded(ReactionAdded reactionAdded)
        {
            if (OnReactionAdded != null)
                OnReactionAdded(reactionAdded, this);
        }

        public void HandleHello(Hello hello)
        {
            HelloReceived = true;

            if (OnHello != null)
                OnHello(hello, this);
        }

        public void HandlePresence(PresenceChange change)
        {
            UserLookup[change.user].Presence = change.presence.ToString().ToLower();
        }

        public void HandleUserChange(UserChange change)
        {
            UserLookup[change.user.Id] = change.user;

            if (OnUserChange != null)
                OnUserChange(change, this);
        }

        public void HandleTeamJoin(TeamJoin newuser)
        {
            UserLookup.Add(newuser.user.Id, newuser.user);
        }

        public void HandleChannelCreated(ChannelCreated created)
        {
            ChannelLookup.Add(created.channel.Id, created.channel);
        }

        public void HandleChannelRename(ChannelRename rename)
        {
            ChannelLookup[rename.channel.Id].Name = rename.channel.Name;

            if (OnChannelRename != null)
                OnChannelRename(rename, this);
        }

        public void HandleChannelDeleted(ChannelDeleted deleted)
        {
            ChannelLookup.Remove(deleted.channel);
        }

        public void HandleChannelArchive(ChannelArchive archive)
        {
            ChannelLookup[archive.channel].IsArchived = true;
        }

        public void HandleChannelUnarchive(ChannelUnarchive unarchive)
        {
            ChannelLookup[unarchive.channel].IsArchived = false;
        }

        public void HandleGroupJoined(GroupJoined joined)
        {
            GroupLookup.Add(joined.channel.Id, joined.channel);
        }

        public void HandleGroupLeft(GroupLeft left)
        {
            GroupLookup.Remove(left.channel.Id);
        }

        public void HandleGroupOpen(GroupOpen open)
        {
            GroupLookup[open.channel].IsOpen = true;
        }

        public void HandleGroupClose(GroupClose close)
        {
            GroupLookup[close.channel].IsOpen = false;
        }

        public void HandleGroupArchive(GroupArchive archive)
        {
            GroupLookup[archive.channel].IsArchived = true;
        }

        public void HandleGroupUnarchive(GroupUnarchive unarchive)
        {
            GroupLookup[unarchive.channel].IsArchived = false;
        }

        public void HandleGroupRename(GroupRename rename)
        {
            GroupLookup[rename.channel.Id].Name = rename.channel.Name;
            GroupLookup[rename.channel.Id].Created = rename.channel.Created;
        }

        public void UserTyping(Typing t)
        {
            if (OnTyping != null)
                OnTyping(t, this);
        }

        public void Message(NewMessage m)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(m, this);
        }

        public void FileShareMessage(FileShareMessage m)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(m, this);
        }

        public void PresenceChange(PresenceChange p)
        {
            if (OnPresenceChange != null)
                OnPresenceChange(p, this);
        }

        /// <summary>
        /// Your channel read marker was updated
        /// </summary>
        /// <param name="m"></param>
        public void ChannelMarked(ChannelMarked m)
        {
            if (OnChannelMarked != null)
                OnChannelMarked(m, this);
        }

        public void ReconnectUrl(ReconnectUrl m)
        {

        }

        public void FilePublic(FilePublic m)
        {

        }

        public void FileShared(FileShared m)
        {

        }

        public void CloseSocket()
        {
            underlyingSocket.Close();
        }
    }
}
