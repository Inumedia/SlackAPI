using System;
using System.Linq;
using System.Threading;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

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
        public void ShouldJoinDirectMessageChannel()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);

            string userName = _config.Slack.DirectMessageUser;
            string user = client.Users.First(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).id;

            // when
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.JoinDirectMessageChannel(response =>
            {
                Assert.IsTrue(response.Ok, "Error while joining user channel");
                Assert.IsTrue(!string.IsNullOrEmpty(response.channel.Id), "We expected a channel id to be returned");
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
    }
}