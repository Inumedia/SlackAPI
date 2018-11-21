using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests.Configuration
{
    public class IntegrationFixture : IDisposable
    {
        private Lazy<SlackSocketClient> userClient;
        private Lazy<SlackSocketClient> botClient;
        private Lazy<SlackTaskClient> userClientAsync;
        private Lazy<SlackTaskClient> botClientAsync;

        public IntegrationFixture()
        {
            this.Config = this.GetConfig();
            this.userClient = new Lazy<SlackSocketClient>(() => this.CreateClient(this.Config.UserAuthToken));
            this.botClient = new Lazy<SlackSocketClient>(() => this.CreateClient(this.Config.BotAuthToken));
            this.userClientAsync = new Lazy<SlackTaskClient>(() => new SlackTaskClient(this.Config.UserAuthToken));
            this.botClientAsync = new Lazy<SlackTaskClient>(() => new SlackTaskClient(this.Config.BotAuthToken));
        }

        public SlackConfig Config { get; }

        public SlackSocketClient UserClient
        {
            get
            {
                Assert.True(userClient.Value.IsReady);
                return userClient.Value;
            }
        }

        public SlackSocketClient BotClient
        {
            get
            {
                Assert.True(botClient.Value.IsReady);
                return botClient.Value;
            }
        }

        public SlackSocketClient CreateUserClient(IWebProxy proxySettings = null)
        {
            return this.CreateClient(this.Config.UserAuthToken, proxySettings);
        }

        public SlackSocketClient CreateBotClient(IWebProxy proxySettings = null)
        {
            return this.CreateClient(this.Config.BotAuthToken, proxySettings);
        }

        public SlackTaskClient UserClientAsync => userClientAsync.Value;

        public SlackTaskClient BotClientAsync => botClientAsync.Value;

        public void Dispose()
        {
            if (this.userClient.IsValueCreated)
            {
                this.UserClient.CloseSocket();
            }

            if (this.botClient.IsValueCreated)
            {
                this.BotClient.CloseSocket();
            }
        }

        private SlackConfig GetConfig()
        {
            var currentAssembly = this.GetType().GetTypeInfo().Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(currentAssembly);
            string fileName = Path.Combine(assemblyDirectory, @"configuration\config.json");
            string json = System.IO.File.ReadAllText(fileName);

            var jsonObject = new {slack = (SlackConfig)null };
            return JsonConvert.DeserializeAnonymousType(json, jsonObject).slack;
        }

        private SlackSocketClient CreateClient(string authToken, IWebProxy proxySettings = null)
        {
            SlackSocketClient client;

            LoginResponse loginResponse = null;
            using (var syncClient = new InSync($"{nameof(SlackClient.Connect)} - Connected callback"))
            using (var syncClientSocket = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected callback"))
            using (var syncClientSocketHello = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected hello callback"))
            {
                client = new SlackSocketClient(authToken, proxySettings);
                client.OnHello += () => syncClientSocketHello.Proceed();
                client.Connect(x =>
                {
                    loginResponse = x;

                    Console.WriteLine($"Connected {x.ok}");
                    syncClient.Proceed();
                    if (!x.ok)
                    {
                        // If connect fails, socket connect callback is not called
                        syncClientSocket.Proceed();
                        syncClientSocketHello.Proceed();
                    }
                }, () =>
                {
                    Console.WriteLine("Socket Connected");
                    syncClientSocket.Proceed();
                });
            }

            loginResponse.AssertOk();

            return client;
        }
    }
}
