﻿using SlackAPI.WebSocketMessages;
using System;
using System.Diagnostics;
using System.Threading;

namespace SlackAPI
{
    public class SlackSocketClient : SlackClient
    {
        SlackSocket underlyingSocket;

        public event EventHandler<MessageEventArgs> OnMessageReceived;
        public event EventHandler<SlackSocketMessageEventArgs> OnHello;

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

        public override void Connect(Action<LoginResponse> onConnected = null, Action onSocketConnected = null)
        {
            if (onConnected == null)
            {
                onConnected = Connected;
            }

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

		public void ConnectSocket(Action onSocketConnected){
            underlyingSocket = new SlackSocket(loginDetails, this, onSocketConnected);
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
			underlyingSocket.Send(new Message() { channel = channelId, text = textData, user = MySelf.id, type = "message" }, new Action<MessageReceived>((mr) => {
				if(onSent != null)
					onSent(mr);
			}));
        }

        public void HandleHello(Hello hello)
        {
            HelloReceived = true;

            StartPing();

            OnHello?.Invoke( this, new SlackSocketMessageEventArgs(hello));
        }

        public void HandlePresence(PresenceChange change)
        {
            UserLookup[change.user].presence = change.presence.ToString().ToLower();
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
            GroupLookup.Remove(left.channel.id);
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

        public void Message(SlackMessage m)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(m));
        }

        public void FileShareMessage(FileShareMessage m)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(m));
        }

        public void Action(ActionMessage m)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(m));
        }

        public void Topic(TopicMessage m)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(m));
        }

        public void PresenceChange(PresenceChange p)
        {

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
