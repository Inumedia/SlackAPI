using System;
using System.Linq;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void ShouldJoinDirectMessageChannel()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            JoinDirectMessageChannelResponse actual = null;

            string userName = _config.Slack.DirectMessageUser;
            string user = client.Users.First(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).id;

            // when
            using (var sync = new InSync(nameof(SlackClient.JoinDirectMessageChannel)))
            {
                client.JoinDirectMessageChannel(response =>
                {
                    actual = response;
                    sync.Proceed();;
                }, user);
            }

            // then
            Assert.IsTrue(actual.ok, "Error while joining user channel");
            Assert.IsTrue(!string.IsNullOrEmpty(actual.channel.id), "We expected a channel id to be returned");
        }
    }
}