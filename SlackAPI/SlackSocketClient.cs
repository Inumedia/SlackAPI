using System.Net.WebSockets;
using System;
using System.Linq;
using System.Net;
using SlackAPI.WebSocketMessages;

namespace SlackAPI
{
    public class SlackSocketClient : SlackClient
    {
        readonly bool maintainPresenceChanges;
        SlackSocket underlyingSocket;

        public event Action<NewMessage> OnMessageReceived;
        public event Action<ReactionAdded> OnReactionAdded;
        public event Action<Pong> OnPongReceived;
        public event Action<PresenceChange> OnPresenceChanged;
        public event Action OnConnectionLost;

        bool HelloReceived;
        public const int PingInterval = 3000;

        public long PingRoundTripMilliseconds { get; private set; }
        public bool IsReady { get { return HelloReceived; } }
        public bool IsConnected { get { return underlyingSocket != null && underlyingSocket.Connected; } }

        public event Action OnHello;
        private LoginResponse loginDetails;

        public SlackSocketClient(string token, IWebProxy proxySettings = null, bool maintainPresenceChanges = false)
            : base(token, proxySettings)
        {
            this.maintainPresenceChanges = maintainPresenceChanges;
        }

        public override void Connect(Action<LoginResponse> onConnected, Action onSocketConnected = null)
        {
            base.Connect((s) => {
                if (s.ok)
                    ConnectSocket(onSocketConnected);

                onConnected(s);
            });
        }

        protected override void Connected(LoginResponse loginDetails)
        {
            this.loginDetails = loginDetails;
            base.Connected(loginDetails);
        }

        public void ConnectSocket(Action onSocketConnected){
            underlyingSocket = new SlackSocket(loginDetails, this, onSocketConnected, this.proxySettings);
            underlyingSocket.ConnectionClosed += UnderlyingSocket_ConnectionClosed;
        }
        
        private void UnderlyingSocket_ConnectionClosed()
        {
            OnConnectionLost?.Invoke();
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
            underlyingSocket.Send(new PresenceChange() { presence = status, user = base.MySelf.id });
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

            if (onSent != null) {
                underlyingSocket.Send( new Message() {channel = channelId, text = textData, user = userName, type = "message"}, onSent);
            } else {
                underlyingSocket.Send(new Message() { channel = channelId, text = textData, user = userName, type = "message" });
            }
        }

        public void SendPing()
        {
            underlyingSocket.Send(new Ping());
        }

        public void SubscribePresenceChange(params string[] usersIds)
        {
            underlyingSocket.Send(new PresenceChangeSubscription(usersIds));
        }

        public void HandlePongReceived(Pong pong)
        {
            if (OnPongReceived != null)
                OnPongReceived(pong);
        }

        public void HandleReactionAdded(ReactionAdded reactionAdded)
        {
            if (OnReactionAdded != null)
                OnReactionAdded(reactionAdded);
        }

        public void HandleHello(Hello hello)
        {
            if (maintainPresenceChanges)
            {
                // Subscribe presence change event for all the users on startup to maintain status in the the lookup table
                SubscribePresenceChange(UserLookup.Keys.ToArray());
            }

            HelloReceived = true;

            if (OnHello != null)
                OnHello();
        }

        public void HandlePresence(PresenceChange change)
        {
            UserLookup[change.user].presence = change.presence.ToString().ToLower();
        }

        public void HandleManualPresence(ManualPresenceChange change)
        {
            change.user = MySelf.id;
            HandlePresence(change);
        }

        public void HandleUserChange(UserChange change)
        {
            UserLookup[change.user.id] = change.user;
        }

        public void HandleTeamJoin(TeamJoin newuser)
        {
            UserLookup.Add(newuser.user.id, newuser.user);
        }

        public void HandleChannelCreated(ChannelCreated created)
        {
            ChannelLookup.Add(created.channel.id, created.channel);
        }

        public void HandleChannelRename(ChannelRename rename)
        {
            ChannelLookup[rename.channel.id].name = rename.channel.name;
        }

        public void HandleChannelDeleted(ChannelDeleted deleted)
        {
            ChannelLookup.Remove(deleted.channel);
        }

        public void HandleChannelArchive(ChannelArchive archive)
        {
            ChannelLookup[archive.channel].is_archived = true;
        }

        public void HandleChannelUnarchive(ChannelUnarchive unarchive)
        {
            ChannelLookup[unarchive.channel].is_archived = false;
        }

        public void HandleGroupJoined(GroupJoined joined)
        {
            GroupLookup.Add(joined.channel.id, joined.channel);
        }

        public void HandleGroupLeft(GroupLeft left)
        {
            GroupLookup.Remove(left.channel);
        }

        public void HandleGroupOpen(GroupOpen open)
        {
            GroupLookup[open.channel].is_open = true;
        }

        public void HandleGroupClose(GroupClose close)
        {
            GroupLookup[close.channel].is_open = false;
        }

        public void HandleGroupArchive(GroupArchive archive)
        {
            GroupLookup[archive.channel].is_archived = true;
        }

        public void HandleGroupUnarchive(GroupUnarchive unarchive)
        {
            GroupLookup[unarchive.channel].is_archived = false;
        }

        public void HandleGroupRename(GroupRename rename)
        {
            GroupLookup[rename.channel.id].name = rename.channel.name;
            GroupLookup[rename.channel.id].created = rename.channel.created;
        }

        public void UserTyping(Typing t)
        {

        }

        public void Message(NewMessage m)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(m);
        }

        public void FileShareMessage(FileShareMessage m)
        {
            Message(m);
        }

        public void PresenceChange(PresenceChange p)
        {
            OnPresenceChanged?.Invoke(p);
        }

        public void ManualPresenceChange(ManualPresenceChange p)
        {
            p.user = MySelf.id;
            PresenceChange(p);
        }

        public void ChannelMarked(ChannelMarked m)
        {

        }

		public void CloseSocket()
		{
			underlyingSocket.Close();
		}
    }
}
