using System;
using System.IO;
using Newtonsoft.Json;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests.Configuration
{
    public class IntegrationFixture : IDisposable
    {
        public IntegrationFixture()
        {
            this.Config = this.GetConfig();
            this.UserClient = this.GetClient(this.Config.UserAuthToken);
            this.BotClient = this.GetClient(this.Config.BotAuthToken);
        }

        public SlackConfig Config { get; }

        public SlackSocketClient UserClient { get; }

        public SlackSocketClient BotClient { get; }

        public void Dispose()
        {
            this.UserClient.CloseSocket();
            this.BotClient.CloseSocket();
        }

        private SlackConfig GetConfig()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
            string json = System.IO.File.ReadAllText(fileName);

            var jsonObject = new {slack = (SlackConfig)null };
            return JsonConvert.DeserializeAnonymousType(json, jsonObject).slack;
        }

        private SlackSocketClient GetClient(string authToken)
        {
            SlackSocketClient client;

            using (var syncClient = new InSync($"{nameof(SlackClient.Connect)} - Connected callback"))
            using (var syncClientSocket = new InSync($"{nameof(SlackClient.Connect)} - SocketConnected callback"))
            {
                client = new SlackSocketClient(authToken);
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
