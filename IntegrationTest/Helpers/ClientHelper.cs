using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using SlackAPI;

namespace IntegrationTest.Helpers
{
    public static class ClientHelper
    {
        public static SlackSocketClient GetClient(string authToken)
        {
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            var client = new SlackSocketClient(authToken);
            client.Connect(x =>
            {
                Console.WriteLine("RTM Start");
            }, () =>
            {
                Console.WriteLine("Connected");
                wait.Set();
            });

            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(wait.WaitOne(), "Still waiting for things to happen...");
                });

            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(client.IsConnected, "Doh, still isn't connected");
                });

            return client;
        }
    }
}