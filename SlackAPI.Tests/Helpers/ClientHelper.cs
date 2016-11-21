using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlackAPI.Tests.Helpers
{
    public static class ClientHelper
    {
        public static SlackSocketClient GetClient(string authToken)
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