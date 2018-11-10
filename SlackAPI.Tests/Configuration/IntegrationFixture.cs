using System;
using System.IO;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests.Configuration
{
    public class IntegrationFixture : IDisposable
    {
        private Lazy<SlackSocketClient> userClient;
        private Lazy<SlackSocketClient> botClient;

        public IntegrationFixture()
        {
            this.Config = this.GetConfig();
            this.userClient = new Lazy<SlackSocketClient>(() => this.GetClient(this.Config.UserAuthToken));
            this.botClient = new Lazy<SlackSocketClient>(() => this.GetClient(this.Config.BotAuthToken));
        }

        public SlackConfig Config { get; }

        public SlackSocketClient UserClient => userClient.Value;

        public SlackSocketClient BotClient => botClient.Value;

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

        private SlackSocketClient GetClient(string authToken)
        {
            SlackSocketClient client;

            using (var syncClient = new InSync($"{nameof(SlackClient.Connect)} - Connected callback"))
            using (var syncClientSocket = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected callback"))
            using (var syncClientSocketHello = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected hello callback"))
            {
                client = new SlackSocketClient(authToken);
                client.OnHello += () => syncClientSocketHello.Proceed();
                client.Connect(x =>
                {
                    Console.WriteLine("Connected");
                    syncClient.Proceed();
                }, () =>
                {
                    Console.WriteLine("Socket Connected");
                    syncClientSocket.Proceed();
                });
            }

            Assert.True(client.IsConnected, "Doh, still isn't connected");

            return client;
        }
    }
}
