using SlackAPI.Models;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SlackAPI
{
    public class SlackSocketClientCollection
    {
        #region Atributes
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        private Dictionary<string, SlackSocketClient> _socketClients;
        public event Action<MessageReceived> OnSent;

        public event Action<NewMessage, SlackSocketClient> OnMessageReceived;
        public event Action<SlackSocketClient> OnConnectionClosed;
        public event Action<Hello, SlackSocketClient> OnHello;
        public event Action<ChannelRename, SlackSocketClient> OnChannelRename;
        public event Action<UserChange, SlackSocketClient> OnUserChange;
        public event Action<PresenceChange, SlackSocketClient> OnPresenceChange;
        public event Action<Typing, SlackSocketClient> OnTyping;
        public event Action<ChannelMarked, SlackSocketClient> OnChannelMarked;
        public event Action<ReactionAdded, SlackSocketClient> OnReactionAdded;
        public event Action<Pong, SlackSocketClient> OnPongReceived;

        public event Action<LoginResponse, SlackClient> OnConnected;
        /// <summary>
        /// Invoke when the loginResponse have the ReasonCode.InvalidAuth or ReasonCode.AccountInactive
        /// T1 is TeamId
        /// T2 is the LoginResponse object
        /// </summary>
        public event Action<string, LoginResponse> OnRemoveSocketClient;

        public SlackSocketClientCollection()
        {
            _socketClients = new Dictionary<string, SlackSocketClient>();
        }

        /// <summary>
        /// Create the connection
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="accessToken"></param>
        public SlackSocketClient CreateSlackClientConnect(string teamId, string accessToken)
        {
            SlackSocketClient client = new SlackSocketClient(accessToken);
            client.Connect(ClientOnConnect);

            client.OnMessageReceived += OnMessageReceived;
            client.OnConnectionClosed += OnConnectionClosed;
            client.OnHello += OnHello;
            client.OnChannelRename += OnChannelRename;
            client.OnUserChange += OnUserChange;
            client.OnPresenceChange += OnPresenceChange;
            client.OnTyping += OnTyping;
            client.OnChannelMarked += OnChannelMarked;
            client.OnReactionAdded += OnReactionAdded;
            client.OnPongReceived += OnPongReceived;

            if (!_socketClients.ContainsKey(teamId))
            {
                lock (_socketClients)
                {
                    _socketClients.Add(teamId, client);
                }
            }

            return client;
        }

        public void ReconnectSlackClient(SlackSocketClient client)
        {
            client.Connect(ClientOnConnect);
        }

        private void ClientOnConnect(LoginResponse loginResponse, SlackClient socketClient)
        {
            if (OnConnected != null)
            {
                OnConnected(loginResponse, socketClient);
                return;
            }

            SlackSocketClient client = (SlackSocketClient)socketClient;

            if (!loginResponse.Ok)
            {
                if (loginResponse.Error == ReasonCode.InvalidAuth || loginResponse.Error == ReasonCode.AccountInactive)
                {
                    foreach (KeyValuePair<string, SlackSocketClient> pair in _socketClients)
                    {
                        if (pair.Value == client)
                        {
                            OnRemoveSocketClient(pair.Key, loginResponse);
                            lock (_socketClients)
                            {
                                _socketClients.Remove(pair.Key);
                            }
                        }
                    }
                }
            }

        }

        public void SendSlackMessage(string teamId, string channelId, string textMessage, string userName = null)
        {
            SlackSocketClient client;
            if (!_socketClients.TryGetValue(teamId, out client))
            {
                Log.Debug("Socket client dont exists on the SocketClients dictionary");
                return;
            }

            client.SendMessage(OnSent, channelId, textMessage, userName);
        }

        public void SendSlackMessage(SlackSocketClient client, string channelId, string textMessage, string userName = null)
        {
            client.SendMessage(OnSent, channelId, textMessage, userName);
        }

        public void Close()
        {
            foreach (KeyValuePair<string, SlackSocketClient> pair in _socketClients)
            {
                if (pair.Value.IsConnected)
                {
                    pair.Value.CloseSocket();
                }

                _socketClients.Remove(pair.Key);
            }
        }
    }
}
