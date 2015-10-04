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
        private Config _config;

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
                if (response != null)
                {

                }
                wait.Set();
            }, user);

            // then
            Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(3)), "Took too long to do the THING");
        }

        private SlackSocketClient Connect()
        {
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            var client = new SlackSocketClient(_config.Slack.AuthToken);
            client.Connect((o) =>
            {
                Debug.WriteLine("RTM Start");
            }, () =>
            {
                Debug.WriteLine("Connected");
                wait.Set();
            });

            Policy
                .Handle<AssertFailedException>()
                .Retry(15)
                .Execute(() =>
                {
                    Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(0.1)));
                    Assert.IsTrue(client.IsConnected);
                });

            return client;
        }
    }
}