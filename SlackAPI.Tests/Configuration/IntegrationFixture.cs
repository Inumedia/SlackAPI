using Newtonsoft.Json;
using SlackAPI.Tests.Helpers;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace SlackAPI.Tests.Configuration
{
    public class IntegrationFixture : IDisposable
    {
        public IntegrationFixture()
        {
            this.Config = this.GetConfig();
            this.UserClient = this.GetClient(this.Config.UserAuthToken);
            this.UserClientWithPresence = this.GetClient(this.Config.UserAuthToken, new[] { new Tuple<string, string>("batch_presence_aware", "true") });
            this.BotClient = this.GetClient(this.Config.BotAuthToken);
        }

        public SlackConfig Config { get; }

        public SlackSocketClient UserClient { get; }

        public SlackSocketClient UserClientWithPresence { get; }

        public SlackSocketClient BotClient { get; }

        public void Dispose()
        {
            this.UserClient.CloseSocket();
            this.UserClientWithPresence.CloseSocket();
            this.BotClient.CloseSocket();
        }

        private SlackConfig GetConfig()
        {
            var currentAssembly = this.GetType().GetTypeInfo().Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(currentAssembly);
            string fileName = Path.Combine(assemblyDirectory, @"configuration\config.json");
            string json = System.IO.File.ReadAllText(fileName);

            var jsonObject = new { slack = (SlackConfig)null };
            return JsonConvert.DeserializeAnonymousType(json, jsonObject).slack;
        }

        private SlackSocketClient GetClient(string authToken, Tuple<string, string>[] loginParameters = null)
        {
            SlackSocketClient client;

            using (var syncClient = new InSync($"{nameof(SlackClient.Connect)} - Connected callback"))
            using (var syncClientSocket = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected callback"))
            {
                client = new SlackSocketClient(authToken, loginParameters);
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
