﻿using SlackAPI.WebSocketMessages;
using System;
using System.Net.WebSockets;

namespace SlackAPI
{
    public class SlackSocketClient : SlackClient
    {
        SlackSocket underlyingSocket;

        public event Action<NewMessage> OnMessageReceived;
        public event Action<ReactionAdded> OnReactionAdded;
        public event Action<Pong> OnPongReceived;
        public event Action<User> OnPresenceChangeReceived;
        public event Action<User> OnUserChangeReceived;

        bool HelloReceived;
        public const int PingInterval = 3000;
        //int pinging;
        //Timer pingingThread;

        public long PingRoundTripMilliseconds { get; private set; }
        public bool IsReady { get { return HelloReceived; } }
        public bool IsConnected { get { return underlyingSocket != null && underlyingSocket.Connected; } }

        public event Action OnHello;
        internal LoginResponse loginDetails;

        public SlackSocketClient(string token, Tuple<string, string>[] loginParameters = null)
            : base(token, loginParameters)
        {
        }

        public override void Connect(Action<LoginResponse> onConnected, Action onSocketConnected = null)
        {
            base.Connect((s) =>
            {
                ConnectSocket(onSocketConnected);
                onConnected(s);
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
        }

        public void ErrorReceiving<K>(Action<WebSocketException> callback)
        {
            if (callback != null) underlyingSocket.ErrorReceiving += callback;
        }

        public void ErrorReceivingDesiralization<K>(Action<Exception> callback)
        {
            if (callback != null) underlyingSocket.ErrorReceivingDesiralization += callback;
        }

        public void ErrorHandlingMessage<K>(Action<Exception> callback)
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

        public void SendPresenceSub(string[] ids)
        {
            underlyingSocket.Send(new PresenceSub() { ids = ids });
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
            else
            {
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
                OnPongReceived(pong);
        }

        public void HandleReactionAdded(ReactionAdded reactionAdded)
        {
            if (OnReactionAdded != null)
                OnReactionAdded(reactionAdded);
        }

        public void HandleHello(Hello hello)
        {
            HelloReceived = true;

            if (OnHello != null)
                OnHello();
        }

        public void HandlePresence(PresenceChange change)
        {
            UserLookup[change.user].presence = change.presence.ToString().ToLower();
            if (OnPresenceChangeReceived != null)
                OnPresenceChangeReceived(UserLookup[change.user]);
        }

        public void HandleUserChange(UserChange change)
        {
            UserLookup[change.user.id] = change.user;
            if (OnUserChangeReceived != null)
                OnUserChangeReceived(UserLookup[change.user.id]);
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

        public void HandleUserTyping(Typing t)
        {
        }

        public void HandleMessage(NewMessage m)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(m);
        }

        public void HandleFileShareMessage(FileShareMessage m)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(m);
        }

        public void HandleChannelMarked(ChannelMarked m)
        {
        }

        public void CloseSocket()
        {
            underlyingSocket.Close();
        }
    }
}
