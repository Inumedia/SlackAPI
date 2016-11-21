using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;

namespace IntegrationTest.Helpers
{
    public static class ClientHelper
    {
        private static Dictionary<string, SlackSocketClient> clientsCache = new Dictionary<string, SlackSocketClient>();

        public static SlackSocketClient GetClient(string authToken)
        {
            SlackSocketClient client;
            if (clientsCache.TryGetValue(authToken, out client) == false)
            {
                client = CreateClient(authToken);
                clientsCache.Add(authToken, client);
            }

            return client;
        }

        private static SlackSocketClient CreateClient(string authToken)
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

            Assert.IsTrue(client.IsConnected, "Doh, still isn't connected");

            return client;
        }
    }
}