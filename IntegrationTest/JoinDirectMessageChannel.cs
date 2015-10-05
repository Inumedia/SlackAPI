using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using IntegrationTest.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using SlackAPI;

namespace IntegrationTest
{
    [TestClass]
    public class JoinDirectMessageChannel
    {
        private readonly Config _config;

        public JoinDirectMessageChannel()
        {
            _config = Config.GetConfig();
        }

        [TestMethod]
        public void should_join_direct_message_channel()
        {
            // given
            var client = Connect();

            string userName = _config.Slack.DirectMessageUser;
            string user = client.Users.First(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).id;

            // when
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.JoinDirectMessageChannel(response =>
            {
                Assert.IsTrue(response.ok, "Error while joining user channel");
                Assert.IsTrue(!string.IsNullOrEmpty(response.channel.id), "We expected a channel id to be returned");
                wait.Set();
            }, user);

            // then
            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(wait.WaitOne(), "Took too long to do the THING");
                });
        }

        private SlackSocketClient Connect()
        {
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            var client = new SlackSocketClient(_config.Slack.AuthToken);
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